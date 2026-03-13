using System.Linq;
using DigitalLove.Game.Levels;
using UnityEngine;

namespace DigitalLove.Game
{
    public class LevelSelector : MonoBehaviour
    {
        [SerializeField] private GameLevelData[] data;

        public GameLevelData GetCurrent(int roundIndex)
        {
            if (data.Length <= roundIndex)
            {
                return data.Last();
            }
            else
            {
                return data[roundIndex];
            }
        }
    }
}