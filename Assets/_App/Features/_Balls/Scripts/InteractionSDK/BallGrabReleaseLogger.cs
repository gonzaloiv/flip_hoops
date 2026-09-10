using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class BallGrabReleaseLogger
    {
        private const float PeriodicLogIntervalSeconds = 0.5f;

        private readonly string ballName;
        private bool enabled;
        private float peakSpeed;
        private float nextPeriodicLogTime;
        private bool forceReleaseLogged;

        public BallGrabReleaseLogger(string ballName)
        {
            this.ballName = ballName;
        }

        public void SetEnabled(bool isEnabled) => enabled = isEnabled;

        public void OnSelect()
        {
            peakSpeed = 0f;
            forceReleaseLogged = false;
            nextPeriodicLogTime = 0f;
            if (!enabled)
                return;

            Debug.Log($"[BallGrab] {ballName} Select");
        }

        public void ObserveSpeed(float speed, float threshold)
        {
            if (speed > peakSpeed)
                peakSpeed = speed;

            if (!enabled || Time.unscaledTime < nextPeriodicLogTime)
                return;

            nextPeriodicLogTime = Time.unscaledTime + PeriodicLogIntervalSeconds;
            Debug.Log(
                $"[BallGrab] {ballName} holding speed={speed:F2} peak={peakSpeed:F2} thr={threshold:F2}");
        }

        public void OnForceRelease(float speed, float threshold)
        {
            if (forceReleaseLogged)
                return;

            forceReleaseLogged = true;
            if (!enabled)
                return;

            Debug.LogWarning(
                $"[BallGrab] {ballName} ForceRelease speed={speed:F2} thr={threshold:F2}");
        }

        public void OnUnselect(float releaseSpeed, float throwSpeed)
        {
            if (!enabled)
                return;

            Debug.Log(
                $"[BallGrab] {ballName} Unselect forced={forceReleaseLogged} " +
                $"release={releaseSpeed:F2} peak={peakSpeed:F2} throw={throwSpeed:F2}");
        }
    }
}
