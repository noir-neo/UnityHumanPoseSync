using System;
using System.Linq;
using UnityEngine;

namespace UnityHumanPoseSync
{
    public readonly struct HumanPose : IEquatable<HumanPose>
    {
        public const int SerializedDataSize = 4 * 3 * (1 + (int)HumanBodyBones.LastBone);

        public Vector3 HipsPosition { get; }
        public Quaternion[] Rotations { get; }

        public HumanPose(Vector3 hipsPosition, Quaternion[] rotations)
        {
            HipsPosition = hipsPosition;
            Rotations = rotations;
        }

        public static byte[] Serialize(HumanPose humanPose)
        {
            return BitConverter.GetBytes(humanPose.HipsPosition.x)
                .Concat(BitConverter.GetBytes(humanPose.HipsPosition.y))
                .Concat(BitConverter.GetBytes(humanPose.HipsPosition.z))
                .Concat(humanPose.Rotations.SelectMany(r =>
                    BitConverter.GetBytes(r.eulerAngles.x)
                        .Concat(BitConverter.GetBytes(r.eulerAngles.y))
                        .Concat(BitConverter.GetBytes(r.eulerAngles.z))))
                .ToArray();
        }

        public static bool TryDeserialize(byte[] data, out HumanPose humanPose)
        {
            if (data.Length != SerializedDataSize)
            {
                humanPose = default;
                return false;
            }

            var hipPosition = new Vector3(BitConverter.ToSingle(data, 0),
                BitConverter.ToSingle(data, 4),
                BitConverter.ToSingle(data, 8));
            var rotations = Enumerable.Range(1, (int) HumanBodyBones.LastBone)
                .Select(i => i * 12)
                .Select(o => Quaternion.Euler(BitConverter.ToSingle(data, o),
                    BitConverter.ToSingle(data, o + 4),
                    BitConverter.ToSingle(data, o + 8)))
                .ToArray();

            humanPose = new HumanPose(hipPosition, rotations);
            return true;
        }

        public bool Equals(HumanPose other)
        {
            return HipsPosition.Equals(other.HipsPosition) && Equals(Rotations, other.Rotations);
        }

        public override bool Equals(object obj)
        {
            return obj is HumanPose other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (HipsPosition.GetHashCode() * 397) ^ (Rotations != null ? Rotations.GetHashCode() : 0);
            }
        }

        public override string ToString()
        {
            return "HipsPosition: " + HipsPosition + ", Rotations: " + (Rotations != null ? string.Join(", ", Rotations) : "null");
        }
    }
}