using DigitalLove.Casual.UI;
using DigitalLove.DataAccess.Leaderboards;
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
        [SerializeField] private HighestScorePosterBehaviour highestScorePosterBehaviour;
        [SerializeField] private FindTheHoopPanel findTheHoopPanel;
        [SerializeField] private ReviewPanel reviewPanel;
        [SerializeField] private LeaderboardPanel leaderboardPanel;

        public void Prepare()
        {
            grabBallPanel.Hide();
            basketSpawner.Hide();
            reviewPanel.Hide();
            leaderboardPanel.Hide();
        }

        public void ShowIntro(Play play, GameLevelData levelData)
        {
            grabBallPanel.Show();
            scoreboardSpawner.ShowRound(play.RoundLabelValue());
            highestScorePosterBehaviour.Show();
            findTheHoopPanel.Show();
            leaderboardPanel.Show();
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
