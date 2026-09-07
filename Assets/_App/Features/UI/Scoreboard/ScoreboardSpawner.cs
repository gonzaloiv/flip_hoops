using DigitalLove.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class ScoreboardSpawner : MonoBehaviour
    {
        [SerializeField] private OnTheWallSpawner onTheWallSpawner;
        [SerializeField] private ScoreboardPanel panel;

        public ScoreboardPanel Panel => panel;

        public void Show(int currentCaseIndex, int totalCases)
        {
            if (!onTheWallSpawner.HasBeenSpawned)
                onTheWallSpawner.Spawn();
            Panel.Show(currentCaseIndex, totalCases);
        }
    }
}