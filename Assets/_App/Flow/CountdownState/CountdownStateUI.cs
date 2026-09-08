using System;
using DigitalLove.Casual.UI;
using DigitalLove.DataAccess;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Levels;
using DigitalLove.Game.UI;
using DigitalLove.Localization;
using UnityEngine;

namespace DigitalLove.Game
{
    public class CountdownStateUI : MonoBehaviour
    {
        private const int MinLevelIndexToShowReviewPanel = 2;
        private const string tableName = "Levels";

        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private WallStackSpawner wallStackSpawner;
        [SerializeField] private GrabBallPanel grabBallPanel;
        [SerializeField] private FindTheHoopPanel findTheHoopPanel;
        [SerializeField] private ReviewPanel reviewPanel;

        public void HideAll()
        {
            grabBallPanel.Hide();
            basketSpawner.Hide();
            reviewPanel.Hide();
        }

        public void ShowIntro(int levelIndex, int totalLevels, int tries)
        {
            grabBallPanel.Show(showVideo: tries == 0);
            if (tries < 1)
                findTheHoopPanel.Show();
            else
                findTheHoopPanel.Hide();
            wallStackSpawner.Show(levelIndex, totalLevels);
            if (levelIndex >= MinLevelIndexToShowReviewPanel)
                reviewPanel.Show();
        }

        public void HideGrabBallPanel() => grabBallPanel.Hide();

        public void SetLevelsInteraction(bool enabled) =>
            wallStackSpawner.LevelsPanel?.SetInteractionEnabled(enabled);

        public void RefreshLevels(LevelSelector levelSelector, PlayerData playerData)
        {
            LevelsPanel panel = wallStackSpawner.LevelsPanel;
            if (panel == null)
                return;
            panel.Refresh(LevelItemDataBuilder.Build(levelSelector, playerData));
        }

        public void SubscribeLevelPressed(Action<string> handler)
        {
            if (wallStackSpawner.LevelsPanel != null)
                wallStackSpawner.LevelsPanel.levelPressed += handler;
        }

        public void UnsubscribeLevelPressed(Action<string> handler)
        {
            if (wallStackSpawner.LevelsPanel != null)
                wallStackSpawner.LevelsPanel.levelPressed -= handler;
        }

        public void ShowBasketInstructions(GameLevelData levelData, int levelIndex)
        {
            string initText;
            string infoText;
            if (!levelData.isCountdownLevel)
            {
                initText = LocalizationUtil.GetValue(tableName: tableName, levelData.IntroKey);
                infoText = string.Empty;
            }
            else
            {
                initText = LocalizationUtil.GetValue(tableName: tableName, "high_score_level_init", levelIndex + 1);
                infoText = LocalizationUtil.GetValue(tableName: tableName, "high_score_level_info");
            }
            basketSpawner.Panel.Show(initText, infoText);
        }

        public void ShowCountdown(int seconds)
        {
            wallStackSpawner.Panel.SetLeftLabel(seconds);
            basketSpawner.Panel.ShowCountdown(seconds);
        }
    }
}
