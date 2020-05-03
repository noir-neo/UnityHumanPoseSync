using UnityEngine;
using UnityHumanPoseSync;

namespace UnityHumanPoseSyncSample
{
    [RequireComponent(typeof(AnimatorHumanPose))]
    public sealed class HumanPoseReceiver : MonoBehaviour
    {
        [SerializeField] int port = 9999;
        [SerializeField] AnimatorHumanPose animatorHumanPose;

        UnityHumanPoseSyncServer server;
        
        bool received;
        UnityHumanPoseSync.HumanPose humanPose;

        void Start()
        {
            server = new UnityHumanPoseSyncServer(port);
            server.ReceiveEvent += (_, args) => OnReceive(args);
            server.Start();
        }

        void OnReceive(UnityHumanPoseSyncServer.ReceiveEventArgs eventArgs)
        {
            humanPose = eventArgs.HumanPose;
            received = true;
        }
        
        void Update()
        {
            if (received)
            {
                animatorHumanPose.Set(humanPose);
                received = false;
            }
        }

        void OnDestroy()
        {
            server?.Stop();
        }

        void Reset()
        {
            animatorHumanPose = GetComponent<AnimatorHumanPose>();
        }
    }
}