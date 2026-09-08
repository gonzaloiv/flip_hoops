using DigitalLove.Global;
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
        [SerializeField] private ScorePanel scorePanel;

        public void Show(string initText, string infoText)
        {
            gameObject.SetActive(true);
            initLabel.text = initText;
            infoLabel.text = infoText;
            infoLabel.transform.parent.gameObject.SetActive(!string.IsNullOrEmpty(infoText));
            this.InvokeAfterSecs(secsActive, () => levelPanel.SetActive(false));
            levelPanel.SetActive(true);
        }

        public void ShowShort(string initText)
        {
            gameObject.SetActive(true);
            initLabel.text = initText;
            infoLabel.transform.parent.gameObject.SetActive(false);
            this.InvokeAfterSecs(secsActive / 2, () => levelPanel.SetActive(false));
            levelPanel.SetActive(true);
        }

        public void ShowScore(int score, bool hasMultiplier)
        {
            scorePanel.Show(score, hasMultiplier);
        }

        public void ShowCountdown(int value, float secsActive = 0.5f)
        {
            scorePanel.ShowCountdown(value, secsActive);
        }

        public void HideAll()
        {
            levelPanel.SetActive(false);
            scorePanel.Hide();
        }
    }
}