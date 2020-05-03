using UnityEngine;
using UnityHumanPoseSync;

namespace UnityHumanPoseSyncSample
{
    [RequireComponent(typeof(AnimatorHumanPose))]
    public sealed class HumanPoseSender : MonoBehaviour
    {
        [SerializeField] string hostname = "127.0.0.1";
        [SerializeField] int port = 9999;
        [SerializeField] AnimatorHumanPose animatorHumanPose;

        UnityHumanPoseSyncClient client;

        void Start()
        {
            client = new UnityHumanPoseSyncClient(hostname, port);
        }

        void LateUpdate()
        {
            client.Send(animatorHumanPose.Get());
        }

        void OnDestroy()
        {
            client?.Dispose();
        }

        void Reset()
        {
            animatorHumanPose = GetComponent<AnimatorHumanPose>();
        }
    }
}