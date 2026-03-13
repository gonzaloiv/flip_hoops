using TMPro;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class ScoreboardPanel : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        [SerializeField] private TextMeshProUGUI scoreLabel;
        [SerializeField] private TextMeshProUGUI timeLabel;
        [SerializeField] private TextMeshProUGUI roundLabel;
        [SerializeField] private AudioSource countdownCompleteSource;
        [SerializeField] private AudioSource lastSecsSource;

        public void Show()
        {
            visuals.SetActive(true);
            SetScore(0);
            timeLabel.text = 0.ToString("00:00");
            SetRound(0);
        }

        public void SetScore(int score)
        {
            scoreLabel.text = score.ToString("00");
        }

        public void SetTime(int time)
        {
            timeLabel.text = time.ToString("00:00");
            if (time <= 3 && time > 0)
                lastSecsSource.Play();
            if (time <= 0)
                countdownCompleteSource.Play();
        }

        public void SetRound(int round)
        {
            roundLabel.text = round.ToString("00");
        }

        public void Hide()
        {
            visuals.SetActive(false);
        }
    }
}