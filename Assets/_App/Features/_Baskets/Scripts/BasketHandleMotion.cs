using UnityEngine;

namespace DigitalLove.Game.Basket
{
    /// <summary>
    /// Ping-pongs local rotation on one axis between two edge angles (e.g. shopping-basket handle).
    /// </summary>
    public class BasketHandleMotion : MonoBehaviour
    {
        public enum Axis
        {
            X = 0,
            Y = 1,
            Z = 2
        }

        [SerializeField] private Axis axis = Axis.X;
        [SerializeField] private float minAngle = -89f;
        [SerializeField] private float maxAngle = 89f;
        [SerializeField] private float degreesPerSecond = 40f;
        [SerializeField] private bool randomizeStart;

        private float phase;
        private Vector3 baseEuler;

        private void OnEnable()
        {
            baseEuler = transform.localEulerAngles;
            float range = Mathf.Abs(maxAngle - minAngle);
            phase = randomizeStart && range > 0f
                ? Random.Range(0f, range * 2f)
                : 0f;
            ApplyAngle(CurrentAngle());
        }

        private void Update()
        {
            float range = Mathf.Abs(maxAngle - minAngle);
            if (range <= 0f || degreesPerSecond <= 0f)
                return;

            phase += degreesPerSecond * Time.deltaTime;
            ApplyAngle(CurrentAngle());
        }

        private float CurrentAngle()
        {
            float range = maxAngle - minAngle;
            return minAngle + Mathf.PingPong(phase, Mathf.Abs(range)) * Mathf.Sign(range);
        }

        private void ApplyAngle(float angle)
        {
            Vector3 euler = baseEuler;
            euler[(int)axis] = angle;
            transform.localRotation = Quaternion.Euler(euler);
        }

        private void OnValidate()
        {
            if (degreesPerSecond < 0f)
                degreesPerSecond = 0f;
        }
    }
}
