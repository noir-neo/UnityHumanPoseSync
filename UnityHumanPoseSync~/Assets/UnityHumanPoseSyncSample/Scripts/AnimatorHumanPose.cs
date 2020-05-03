using System;
using System.Linq;
using UnityEngine;

namespace UnityHumanPoseSyncSample
{
    [RequireComponent(typeof(Animator))]
    public sealed class AnimatorHumanPose : MonoBehaviour
    {
        [SerializeField] Animator _animator;
        [SerializeField] Transform[] humanBodyBones;

        public void Set(UnityHumanPoseSync.HumanPose humanPose)
        {
            humanBodyBones[(int) HumanBodyBones.Hips].position = humanPose.HipsPosition;
            foreach (var (transform, rotation) in humanBodyBones.Zip(humanPose.Rotations, (t, r) => (t, r)))
            {
                if (transform == null) continue;
                transform.rotation = rotation;
            }
        }

        public UnityHumanPoseSync.HumanPose Get()
        {
            var hipsPosition = humanBodyBones[(int) HumanBodyBones.Hips].position;
            var rotations = humanBodyBones.Select(t => t != null ? t.rotation : Quaternion.identity).ToArray();
            return new UnityHumanPoseSync.HumanPose(hipsPosition, rotations);
        }

        void Reset()
        {
            _animator = GetComponent<Animator>();
            if (_animator == null || !_animator.isHuman) return;
            humanBodyBones = Enum.GetValues(typeof(HumanBodyBones))
                .Cast<HumanBodyBones>()
                .Take((int)HumanBodyBones.LastBone)
                .Select(x => _animator.GetBoneTransform(x))
                .ToArray();
        }
    }
}