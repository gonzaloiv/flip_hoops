using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DigitalLove.Game.UI
{
    public class LevelItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI identityLabel;
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private TextMeshProUGUI statusLabel;
        [SerializeField] private Button button;
        [SerializeField] private CanvasGroup canvasGroup;

        private string levelId;
        private Action<string> onPressed;
        private bool pressesEnabled;
        private bool locked;

        private void OnEnable() => button.onClick.AddListener(OnClick);

        private void OnDisable() => button.onClick.RemoveListener(OnClick);

        public void Bind(LevelItemData data, Action<string> onPressed, bool pressesEnabled)
        {
            levelId = data.levelId;
            this.onPressed = onPressed;
            this.pressesEnabled = pressesEnabled;
            locked = data.locked;
            identityLabel.text = data.identityLabel;
            scoreLabel.text = data.scoreText;
            statusLabel.text = BuildStatusText(data);
            ApplyInteractable();
        }

        private static string BuildStatusText(LevelItemData data)
        {
            if (data.locked)
                return "LOCK";
            if (data.selected && data.frontier)
                return "SEL*";
            if (data.selected)
                return "SEL";
            if (data.frontier)
                return "NEXT";
            if (data.passed)
                return "PASS";
            return string.Empty;
        }

        private void ApplyInteractable()
        {
            bool canPress = pressesEnabled && !locked;
            button.interactable = canPress;
            if (canvasGroup != null)
                canvasGroup.alpha = LockedAlpha();
        }

        private float LockedAlpha() => locked ? 0.45f : 1f;

        private void OnClick()
        {
            if (!pressesEnabled || locked)
                return;
            onPressed?.Invoke(levelId);
        }
    }
}
