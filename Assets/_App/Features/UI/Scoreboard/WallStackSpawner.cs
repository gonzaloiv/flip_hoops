using DigitalLove.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class WallStackSpawner : MonoBehaviour
    {
        [SerializeField] private OnTheWallSpawner onTheWallSpawner;
        [SerializeField] private ScoreboardPanel panel;
        [SerializeField] private LevelsPanel levelsPanel;

        public ScoreboardPanel Panel => panel;
        public LevelsPanel LevelsPanel => levelsPanel;

        public void Show(int currentCaseIndex, int totalCases)
        {
            if (!onTheWallSpawner.HasBeenSpawned)
                onTheWallSpawner.Spawn();
            Panel.Show(currentCaseIndex, totalCases);
            levelsPanel?.Show();
        }
    }
}
