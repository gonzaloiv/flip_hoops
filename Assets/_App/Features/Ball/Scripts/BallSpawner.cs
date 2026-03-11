using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Ball
{
    public class BallSpawner : MonoBehaviour
    {
        [SerializeField] private FindSpawnPositions findSpawnPositions;

        public void Spawn()
        {
            findSpawnPositions.StartSpawn();
        }
    }
}