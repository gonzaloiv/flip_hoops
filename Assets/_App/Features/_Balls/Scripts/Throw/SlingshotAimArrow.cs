using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class SlingshotAimArrow : MonoBehaviour
    {
        [SerializeField] private LineRenderer line;
        [SerializeField] private float minPullDistance = 0.06f;
        [SerializeField] private float maxPullDistance = 0.65f;
        [SerializeField] private float minLength = 0.25f;
        [SerializeField] private float maxLength = 1.2f;
        [SerializeField] private float headSize = 0.1f;

        public void Show()
        {
            if (line == null)
                return;
            line.positionCount = 5;
            line.enabled = true;
        }

        public void Hide()
        {
            if (line != null)
                line.enabled = false;
        }

        public void Draw(Vector3 origin, Vector3 ball)
        {
            if (line == null || !line.enabled)
                return;
            Vector3 toOrigin = origin - ball;
            float pull = toOrigin.magnitude;
            if (pull < 0.001f)
                return;
            SetArrow(origin, toOrigin / pull, ArrowLength(pull));
        }

        private float ArrowLength(float pull)
        {
            float t = Mathf.InverseLerp(minPullDistance, maxPullDistance, pull);
            return Mathf.Lerp(minLength, maxLength, t);
        }

        private void SetArrow(Vector3 origin, Vector3 dir, float length)
        {
            Vector3 tip = origin + dir * length;
            Vector3 right = HeadRight(dir) * (headSize * 0.5f);
            Vector3 back = tip - dir * headSize;
            line.SetPosition(0, origin);
            line.SetPosition(1, tip);
            line.SetPosition(2, back + right);
            line.SetPosition(3, tip);
            line.SetPosition(4, back - right);
        }

        private static Vector3 HeadRight(Vector3 dir)
        {
            Vector3 right = Vector3.Cross(dir, Vector3.up);
            if (right.sqrMagnitude < 0.0001f)
                right = Vector3.Cross(dir, Vector3.right);
            return right.normalized;
        }
    }
}
