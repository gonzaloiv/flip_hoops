using DigitalLove.Casual.UI;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using DigitalLove.Game.UI;
using DigitalLove.Localization;
using UnityEngine;
using DigitalLove.Casual.Flow;

namespace DigitalLove.Game
{
    public class CountdownStateUI : MonoBehaviour
    {
        [SerializeField] private string tableName = "Levels";
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ScoreboardSpawner scoreboardSpawner;
        [SerializeField] private GrabBallPanel grabBallPanel;
        [SerializeField] private FindTheHoopPanel findTheHoopPanel;
        [SerializeField] private ReviewPanel reviewPanel;

        public void HideAll()
        {
            grabBallPanel.Hide();
            basketSpawner.Hide();
            reviewPanel.Hide();
        }

        public void ShowIntro(Play play)
        {
            grabBallPanel.Show();
            scoreboardSpawner.ShowRound(play.RoundLabelValue());
            if (play.Tries >= 1) // ? Show review panel after warm up
                reviewPanel.Show();
        }

        public void HideGrabBallPanel() => grabBallPanel.Hide();

        public void ShowBasketInstructions(GameLevelData levelData, Play play)
        {
            string initText;
            string infoText;
            if (levelData.IsWarmUp)
            {
                initText = LocalizationUtil.GetValue(tableName: tableName, levelData.IntroKey);
                infoText = LocalizationUtil.GetValue(tableName: tableName, levelData.InfoKey, levelData.basketsToScore);
            }
            else
            {
                initText = LocalizationUtil.GetValue(tableName: tableName, "high_score_level_init", play.RoundLabelValue());
                infoText = LocalizationUtil.GetValue(tableName: tableName, "high_score_level_info");
            }
            basketSpawner.Panel.Show(initText, infoText);
        }

        public void ShowCountdown(int seconds)
        {
            scoreboardSpawner.Panel.SetTime(seconds);
            basketSpawner.Panel.ShowCountdown(seconds);
        }
    }
}
