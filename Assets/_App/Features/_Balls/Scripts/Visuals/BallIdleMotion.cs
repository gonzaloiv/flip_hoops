using DG.Tweening;
using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class BallIdleMotion : MonoBehaviour
    {
        [SerializeField] private Transform content;
        [SerializeField] private float minCycleSecs = 1.5f;
        [SerializeField] private float maxCycleSecs = 2.5f;
        [SerializeField] private float punchSecs = 0.25f;
        [SerializeField] private float punchMultiplier = 0.12f;
        [SerializeField] private Vector3 degreesPerSecond = new(12f, 40f, 8f);

        private Vector3 baseScale;
        private Quaternion baseLocalRotation;
        private bool hasBase;
        private bool isIdle;
        private float countdown;

        private void OnEnable()
        {
            CacheBase();
            isIdle = true;
            countdown = 0f;
        }

        private void OnDisable() => StopIdle();

        public void StopIdle()
        {
            isIdle = false;
            if (content == null || !hasBase)
                return;

            content.DOKill();
            content.localScale = baseScale;
            content.localRotation = baseLocalRotation;
        }

        private void Update()
        {
            if (!isIdle || content == null)
                return;

            content.Rotate(degreesPerSecond * Time.deltaTime, Space.Self);
            countdown -= Time.deltaTime;
            if (countdown > 0f)
                return;

            Punch();
            countdown = Random.Range(minCycleSecs, maxCycleSecs);
        }

        private void Punch()
        {
            content.DOKill();
            content.localScale = baseScale;
            content.DOPunchScale(baseScale * punchMultiplier, punchSecs);
        }

        private void CacheBase()
        {
            if (content == null || hasBase)
                return;

            baseScale = content.localScale;
            baseLocalRotation = content.localRotation;
            hasBase = true;
        }
    }
}
