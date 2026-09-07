using TMPro;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class ScoreboardPanel : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private TextMeshProUGUI topLabel;
        [SerializeField] private TextMeshProUGUI leftLabel;
        [SerializeField] private TextMeshProUGUI rightLabel;
        [SerializeField] private AudioSource countdownCompleteSource;
        [SerializeField] private AudioSource lastSecsSource;

        public void Show(int currentCaseIndex, int totalCases)
        {
            visuals.SetActive(true);
            SetTopLabel(currentCaseIndex, totalCases);
            SetLeftLabel(0);
            SetRightLabel(0);
        }

        private void SetTopLabel(int currentCaseIndex, int totalCases)
        {
            topLabel.text = $"{currentCaseIndex + 1:00}/{totalCases:00}";
        }

        public void SetLeftLabel(int throws)
        {
            leftLabel.text = throws.ToString("00");
        }

        public void SetRightLabel(int score)
        {
            rightLabel.text = score.ToString("00");
        }

        public void Hide()
        {
            visuals.SetActive(false);
        }
    }
}