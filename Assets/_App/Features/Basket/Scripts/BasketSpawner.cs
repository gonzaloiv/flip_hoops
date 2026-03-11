using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class BasketSpawner : MonoBehaviour
    {
        [SerializeField] private FindSpawnPositions findSpawnPositions;

        public void Spawn()
        {
            findSpawnPositions.StartSpawn();
        }
    }
}