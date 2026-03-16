using DigitalLove.XR;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class ScoreboardSpawner : MonoBehaviour
    {
        [SerializeField] private OnTheWallSpawner onTheWallSpawner;
        [SerializeField] private ScoreboardPanel panel;

        public ScoreboardPanel Panel => panel;

        public void ShowRound(int value)
        {
            if (!onTheWallSpawner.HasBeenSpawned) 
                onTheWallSpawner.Spawn();
            Panel.Show();
            Panel.SetRound(value);
        }
    }
}