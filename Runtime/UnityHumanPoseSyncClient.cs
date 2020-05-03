using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace UnityHumanPoseSync
{
    public sealed class UnityHumanPoseSyncClient : IDisposable
    {
        readonly TcpClient client;
        readonly NetworkStream stream;

        public UnityHumanPoseSyncClient(string hostname, int port)
        {
            client = new TcpClient(hostname, port);
            Debug.Log($"Connected Server({((IPEndPoint)client.Client.RemoteEndPoint)?.Address}:{((IPEndPoint)client.Client.RemoteEndPoint)?.Port}) ({((IPEndPoint)client.Client.LocalEndPoint).Address}:{((IPEndPoint)client.Client.LocalEndPoint).Port})");
            stream = client.GetStream();
        }

        public void Send(HumanPose humanPose)
        {
            var data = HumanPose.Serialize(humanPose);
            stream.Write(data, 0, data.Length);
            Debug.Log($"Sent: {humanPose}");
        }

        public void Dispose()
        {
            stream.Close();
            client.Close();
        }
    }
}