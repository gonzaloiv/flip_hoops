using DigitalLove.VFX;
using DigitalLove.Global;
using DigitalLove.Localization;
using TMPro;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class BasketPanel : MonoBehaviour
    {
        [Header("Level Info")]
        [SerializeField] private TextMeshProUGUI initLabel;
        [SerializeField] private TextMeshProUGUI infoLabel;
        [SerializeField] private GameObject levelPanel;
        [SerializeField] private float secsActive = 5;

        [Header("Score")]
        [SerializeField] private ScalePunch scalePunch;
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private float scoreSecsActive = 2;

        public void Show(string initText, string infoText)
        {
            gameObject.SetActive(true);
            initLabel.text = initText;
            infoLabel.text = infoText;
            this.InvokeAfterSecs(secsActive, () => levelPanel.SetActive(false));
            levelPanel.SetActive(true);
            scoreLabel.enabled = false;
        }

        public void ShowScore(int score)
        {
            levelPanel.SetActive(false);
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
            levelPanel.SetActive(false);
            scoreLabel.enabled = false;
        }
    }
}