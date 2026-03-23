using DigitalLove.FX;
using DigitalLove.Global;
using DigitalLove.Localization;
using TMPro;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class BasketPanel : MonoBehaviour
    {
        [Header("Main")]
        [SerializeField] private TextMeshProUGUI initLabel;
        [SerializeField] private TextMeshProUGUI infoLabel;
        [SerializeField] private float secsActive = 5;
        [SerializeField] private GameObject content;
        [SerializeField] private ScalePunch scalePunch;

        [Header("Score")]
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private float scoreSecsActive = 2;

        public void Show(string initText, string infoText)
        {
            gameObject.SetActive(true);
            initLabel.text = initText;
            infoLabel.text = infoText;
            this.InvokeAfterSecs(secsActive, () => content.SetActive(false));
            content.SetActive(true);
            scoreLabel.enabled = false;
        }

        public void ShowScore(int score)
        {
            content.SetActive(false);
            scalePunch.Animate();
            scoreLabel.text = $"+{score}";
            scoreLabel.enabled = true;
            this.InvokeAfterSecs(scoreSecsActive, () => scoreLabel.enabled = false);
        }

        public void ShowCountdown(int value, float secsActive = 0.5f)
        {
            scalePunch.Animate();
            scoreLabel.text = value > 0 ? value.ToString() : LocalizationUtil.GetValue("go");
            scoreLabel.enabled = true;
            this.InvokeAfterSecs(secsActive, () => scoreLabel.enabled = false);
        }

        public void HideAll()
        {
            content.SetActive(false);
            scoreLabel.enabled = false;
        }
    }
}