using System;
using DigitalLove.Game.Basket;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class BasketModifierBehaviour : ModifierBehaviour
    {
        [SerializeField] private BasketBehaviour basket;
        [SerializeField] private ScorePanel scorePanel;
        [SerializeField] private float multiplier = 1.5f;
        [SerializeField] private float xOffset;
        [SerializeField] private float yOffset;

        public BasketBehaviour Basket => basket;

        public override void Init(Transform throwZoneTransform, Transform basketTransform, Action<int> scored)
        {
            base.Init(throwZoneTransform, basketTransform, scored);
            SetOriginRelatedPosition(throwZoneTransform, basketTransform);
            scorePanel.transform.position = basket.PanelRef.position;
            scorePanel.Hide();
        }

        private void SetOriginRelatedPosition(Transform throwZoneTransform, Transform basketTransform)
        {
            basket.transform.SetParent(basketTransform);
            basket.transform.localPosition = Vector3.zero;
            basket.transform.localRotation = Quaternion.identity;
            basket.transform.localPosition += throwZoneTransform.right * xOffset;
            basket.transform.localPosition += -throwZoneTransform.forward * yOffset;
            basket.transform.parent = transform;
        }

        private void OnEnable()
        {
            basket.scored.AddListener(OnScored);
        }

        private void OnScored(int score)
        {
            score = (int)(multiplier * score);
            scored.Invoke(score);
        }

        private void OnDisable()
        {
            basket.scored.RemoveListener(OnScored);
        }

        public override void ShowScore(int score, bool hasMultiplier)
        {
            scorePanel.Show(score, hasMultiplier);
        }
    }
}
