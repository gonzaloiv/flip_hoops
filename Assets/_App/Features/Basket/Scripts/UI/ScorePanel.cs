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

        public void Hide() => gameObject.SetActive(false);

        public void Show(int score)
        {
            gameObject.SetActive(true);
            scoreLabel.text = $"+{score}";
            scalePunch.Animate();
            this.InvokeAfterSecs(scoreSecsActive, () => gameObject.SetActive(false));
        }

        public void ShowCountdown(int value, float secsActive = 0.5f)
        {
            gameObject.SetActive(true);
            scalePunch.Animate();
            scoreLabel.text = value > 0 ? value.ToString() : LocalizationUtil.GetValue("go");
            scoreLabel.enabled = true;
            this.InvokeAfterSecs(secsActive, () => gameObject.SetActive(false));
        }
    }
}