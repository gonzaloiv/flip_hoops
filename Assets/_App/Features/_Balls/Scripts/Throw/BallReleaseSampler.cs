using System.Collections.Generic;
using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class BallReleaseSampler
    {
        private readonly Queue<Vector3> linearSamples = new();
        private readonly Queue<Vector3> angularSamples = new();
        private Vector3 previousPosition;
        private Quaternion previousRotation = Quaternion.identity;
        private bool hasPrevious;

        public void Clear()
        {
            linearSamples.Clear();
            angularSamples.Clear();
            hasPrevious = false;
        }

        public void Sample(Transform target, float deltaTime, int maxSamples)
        {
            if (deltaTime <= 0f)
                return;

            if (!hasPrevious)
            {
                CachePose(target);
                return;
            }

            Enqueue(linearSamples, (target.position - previousPosition) / deltaTime, maxSamples);
            Enqueue(angularSamples, AngularVelocity(previousRotation, target.rotation, deltaTime), maxSamples);
            CachePose(target);
        }

        public BallThrowRelease Resolve()
        {
            return new BallThrowRelease(
                WeightedAverage(linearSamples),
                WeightedAverage(angularSamples));
        }

        private void CachePose(Transform target)
        {
            previousPosition = target.position;
            previousRotation = target.rotation;
            hasPrevious = true;
        }

        private static void Enqueue(Queue<Vector3> samples, Vector3 value, int maxSamples)
        {
            samples.Enqueue(value);
            while (samples.Count > maxSamples)
                samples.Dequeue();
        }

        private static Vector3 AngularVelocity(Quaternion from, Quaternion to, float deltaTime)
        {
            Quaternion delta = to * Quaternion.Inverse(from);
            delta.ToAngleAxis(out float angleDegrees, out Vector3 axis);
            if (angleDegrees > 180f)
                angleDegrees -= 360f;
            if (axis.sqrMagnitude < 0.0001f)
                return Vector3.zero;
            return axis.normalized * (angleDegrees * Mathf.Deg2Rad / deltaTime);
        }

        private static Vector3 WeightedAverage(Queue<Vector3> samples)
        {
            if (samples.Count == 0)
                return Vector3.zero;

            Vector3 total = Vector3.zero;
            float weightSum = 0f;
            int weight = 1;
            foreach (Vector3 sample in samples)
            {
                total += sample * weight;
                weightSum += weight;
                weight++;
            }

            return total / weightSum;
        }
    }
}
