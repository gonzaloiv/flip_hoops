using System;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class BasketSpawner : MonoBehaviour
    {
        [SerializeField] private FindSpawnPositions findSpawnPositions;

        public Action scored = () => { };

        public void Spawn()
        {
            findSpawnPositions.StartSpawn();
            foreach (GameObject go in findSpawnPositions.SpawnedObjects)
                go.GetComponent<BasketBehaviour>().scored.AddListener(OnBasketScored);
        }

        private void OnBasketScored()
        {
            scored.Invoke();
        }

        public void Unspawn()
        {
            foreach (GameObject go in findSpawnPositions.SpawnedObjects)
                go.GetComponent<BasketBehaviour>().scored.RemoveListener(OnBasketScored);
            findSpawnPositions.ClearSpawnedPrefabs();
        }
    }
}