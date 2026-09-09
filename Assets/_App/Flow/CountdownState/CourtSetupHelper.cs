using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using DigitalLove.Game.Levels;
using DigitalLove.Game.UI;
using UnityEngine;
using DigitalLove.Audio;
using DigitalLove.Casual.Flow;
using System;
using Meta.XR.MRUtilityKit;
using System.Collections;

namespace DigitalLove.Game
{
    // Used for round init, for access to component elements direct references 
    public class CourtSetupHelper : MonoBehaviour
    {
        private const int MaxAttempts = 5;

        [SerializeField] private BallsSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ThrowZone throwZone;
        [SerializeField] private PosterBehaviour[] posters;
        [SerializeField] private TheRadioBehaviour theRadioBehaviour;
        [SerializeField] private EffectMesh floorMesh;

        public float DistanceToCamera =>
            Vector3.Distance(basketSpawner.Basket.WorldPosition, Camera.main.transform.position);

        public void Init()
        {
            theRadioBehaviour.SetActive(false);
        }

        public void Spawn(GameLevelData levelData, Play play, Action onComplete)
        {
            Vector3 gravityDirection = TrySpawnBasket(levelData);
            posters.Spawn(gravityDirection);
            throwZone.SetReference(basketSpawner.Basket.transform);
            ballSpawner.Spawn(levelData.ball, gravityDirection, basketSpawner.Basket.transform);
            SpawnRadioIfFirstTry(play, onComplete);
        }

        private void SpawnRadioIfFirstTry(Play play, Action onComplete)
        {
            if (play.Tries == 0)
            {
                IEnumerator SpawnRoutine()
                {
                    yield return new WaitUntil(HasFloorAtSpawnPoint);
                    theRadioBehaviour.SetSpawnPointPosition();
                    yield return new WaitForFixedUpdate();
                    theRadioBehaviour.SetActive(true);
                    onComplete.Invoke();
                }
                StartCoroutine(SpawnRoutine());
            }
            else
            {
                onComplete.Invoke();
            }
        }

        private bool HasFloorAtSpawnPoint()
        {
            if (floorMesh.EffectMeshObjects.Count == 0)
                return false;

            Vector3 origin = throwZone.WorldPosition + Vector3.up * 0.5f;
            return Physics.Raycast(origin, Vector3.down, 1f);
        }

        private Vector3 TrySpawnBasket(GameLevelData levelData)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                throwZone.Spawn();
                Vector3 gravityDirection = basketSpawner.SpawnAndGetGravityDirection(
                    levelData.basket,
                    levelData.gravity,
                    throwZone.transform,
                    levelData.distance.minMax);
                if (gravityDirection != Vector3.zero)
                    return gravityDirection;

                throwZone.Unspawn();
                basketSpawner.Hide();
            }

            return Vector3.zero;
        }

        public void Clear()
        {
            ballSpawner.Unspawn();
            throwZone.Unspawn();
            basketSpawner.Hide();
        }
    }
}
