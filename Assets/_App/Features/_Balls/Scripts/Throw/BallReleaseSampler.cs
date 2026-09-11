using System.Collections.Generic;
using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class BallReleaseSampler
    {
        private const float MinSampleDelta = 1f / 120f;
        private const float MaxSampleDelta = 1f / 30f;
        private const float PeakCapFactor = 1.35f;
        private const float PeakBlend = 0.4f;

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
            deltaTime = Mathf.Clamp(deltaTime, MinSampleDelta, MaxSampleDelta);
            if (!hasPrevious)
            {
                CachePose(target);
                return;
            }

            Enqueue(linearSamples, (target.position - previousPosition) / deltaTime, maxSamples);
            Enqueue(angularSamples, AngularVelocity(previousRotation, target.rotation, deltaTime), maxSamples);
            CachePose(target);
        }

        public BallThrowRelease Resolve(int trailingSamplesToIgnore = 2)
        {
            return new BallThrowRelease(
                ResolveLinear(trailingSamplesToIgnore),
                WeightedAverage(angularSamples, trailingSamplesToIgnore));
        }

        private void CachePose(Transform target)
        {
            previousPosition = target.position;
            previousRotation = target.rotation;
            hasPrevious = true;
        }

        private Vector3 ResolveLinear(int trailingSamplesToIgnore)
        {
            Vector3 average = WeightedAverage(linearSamples, trailingSamplesToIgnore);
            Vector3 peak = PeakSample(linearSamples, trailingSamplesToIgnore);
            peak = CapPeak(peak, average);
            if (peak.sqrMagnitude <= average.sqrMagnitude)
                return average;
            return Vector3.Lerp(average, peak, PeakBlend);
        }

        private static Vector3 CapPeak(Vector3 peak, Vector3 average)
        {
            float averageSpeed = average.magnitude;
            if (averageSpeed < 0.01f)
                return peak;

            float maxPeakSpeed = averageSpeed * PeakCapFactor;
            float peakSpeed = peak.magnitude;
            if (peakSpeed <= maxPeakSpeed)
                return peak;
            return peak * (maxPeakSpeed / peakSpeed);
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

        private static Vector3 PeakSample(Queue<Vector3> samples, int trailingSamplesToIgnore)
        {
            int usable = UsableCount(samples.Count, trailingSamplesToIgnore);
            if (usable <= 0)
                return Vector3.zero;

            Vector3 peak = Vector3.zero;
            float peakSqr = 0f;
            int index = 0;
            foreach (Vector3 sample in samples)
            {
                if (index >= usable)
                    break;
                float sqr = sample.sqrMagnitude;
                if (sqr > peakSqr)
                {
                    peakSqr = sqr;
                    peak = sample;
                }
                index++;
            }

            return peak;
        }

        private static Vector3 WeightedAverage(Queue<Vector3> samples, int trailingSamplesToIgnore = 0)
        {
            int usable = UsableCount(samples.Count, trailingSamplesToIgnore);
            if (usable <= 0)
                return Vector3.zero;

            Vector3 total = Vector3.zero;
            float weightSum = 0f;
            int weight = 1;
            int index = 0;
            foreach (Vector3 sample in samples)
            {
                if (index >= usable)
                    break;
                total += sample * weight;
                weightSum += weight;
                weight++;
                index++;
            }

            return total / weightSum;
        }

        private static int UsableCount(int sampleCount, int trailingSamplesToIgnore)
        {
            if (sampleCount <= 0)
                return 0;
            if (trailingSamplesToIgnore <= 0)
                return sampleCount;
            return Mathf.Max(1, sampleCount - trailingSamplesToIgnore);
        }
    }
}
