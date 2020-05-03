using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Utils;
using System.Linq;

namespace UnityHumanPoseSync.Tests.Editor
{
    public sealed class HumanPoseTest
    {
        [Test]
        public void SerializeAndDeserializeTest()
        {
            var hipPosition = Random.insideUnitSphere;
            var rotations = Enumerable.Range(0, 55).Select(_ => Random.rotation).ToArray();
            var humanPose = new HumanPose(hipPosition, rotations);
            var serialized = HumanPose.Serialize(humanPose);
            var result = HumanPose.TryDeserialize(serialized, out var deserialized);
            Assert.IsTrue(result);
            Assert.That(deserialized.HipsPosition, Is.EqualTo(humanPose.HipsPosition).Using(Vector3EqualityComparer.Instance));
            Assert.That(deserialized.Rotations.Length, Is.EqualTo(humanPose.Rotations.Length));
            foreach (var (a, e) in deserialized.Rotations.Zip(humanPose.Rotations, (a, e) => (a, e)))
            {
                Assert.That(a, Is.EqualTo(e).Using(QuaternionEqualityComparer.Instance));
            }
        }
    }
}
