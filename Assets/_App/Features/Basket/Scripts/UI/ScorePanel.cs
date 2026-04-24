using System;
using DigitalLove.Global;
using DigitalLove.Localization;
using DigitalLove.VFX;
using TMPro;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class ScorePanel : MonoBehaviour
    {
        [Header("Score")]
        [SerializeField] private ScalePunch scalePunch;
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private float scoreSecsActive = 1.33f;
        [SerializeField] private ColorValue regularTextColor;
        [SerializeField] private ColorValue multiplierTextColor;

        public void Hide() => gameObject.SetActive(false);

        public void Show(int score, bool hasMultiplier)
        {
            gameObject.SetActive(true);
            scoreLabel.text = $"+{score}";
            scalePunch.Animate();
            scoreLabel.color = hasMultiplier ? multiplierTextColor.value : regularTextColor.value;
            this.InvokeAfterSecs(scoreSecsActive, () => gameObject.SetActive(false));
        }

        public void ShowCountdown(int value, float secsActive = 0.5f)
        {
            gameObject.SetActive(true);
            scalePunch.Animate();
            scoreLabel.text = value > 0 ? value.ToString() : LocalizationUtil.GetValue("go");
            scoreLabel.enabled = true;
            scoreLabel.color = regularTextColor.value;
            this.InvokeAfterSecs(secsActive, () => gameObject.SetActive(false));
        }
    }
}