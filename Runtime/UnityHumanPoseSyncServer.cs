using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace UnityHumanPoseSync
{
    public sealed class UnityHumanPoseSyncServer
    {
        readonly TcpListener listener;
        volatile bool listening;

        static readonly IPAddress localaddr = IPAddress.Any;

        public event ReceiveEventHandler ReceiveEvent;
        public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs args);
        public sealed class ReceiveEventArgs
        {
            public HumanPose HumanPose { get; }

            public ReceiveEventArgs(HumanPose humanPose)
            {
                HumanPose = humanPose;
            }
        }

        public UnityHumanPoseSyncServer(int port)
        {
            listener = new TcpListener(localaddr, port);
        }

        public void Start()
        {
            listening = true;
            listener.Start();
            Debug.Log($"Start Server ({((IPEndPoint)listener.LocalEndpoint).Address}:{((IPEndPoint)listener.LocalEndpoint).Port})。");
            Task.Run(ListenAsync);
        }

        public void Stop()
        {
            listening = false;
            listener.Stop();
        }

        async Task ListenAsync()
        {
            try
            {
                while (listening)
                {
                    var client = await listener.AcceptTcpClientAsync();
                    Debug.Log($"Accepted Client {((IPEndPoint) client.Client.LocalEndPoint).Address}:{((IPEndPoint) client.Client.LocalEndPoint).Port})");
                    using (client)
                    {
                        using (var stream = client.GetStream())
                        {
                            var data = new byte[HumanPose.SerializedDataSize];
                            while (listening && await stream.ReadAsync(data, 0, data.Length) != 0)
                            {
                                if (HumanPose.TryDeserialize(data, out var humanPose))
                                {
                                    ReceiveEvent?.Invoke(this, new ReceiveEventArgs(humanPose));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
            finally
            {
                listener.Stop();
            }
        }
    }
}