using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using DigitalLove.Game.Levels;
using DigitalLove.Game.Modifiers;
using DigitalLove.Game.Obstacles;
using DigitalLove.Game.UI;
using UnityEngine;
using DigitalLove.Audio;
using DigitalLove.Casual.Flow;
using System;
using Meta.XR.MRUtilityKit;
using System.Collections;

namespace DigitalLove.Game
{
    public class CourtSetupHelper : MonoBehaviour
    {
        private const int MaxAttempts = 5;

        [SerializeField] private BallsSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ObstacleSpawner obstacleSpawner;
        [SerializeField] private ModifierSpawner modifierSpawner;
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
            Vector3 gravityDirection = TrySpawnCourt(levelData);
            posters.Spawn(gravityDirection);
            throwZone.SetReference(basketSpawner.Basket.transform);
            ballSpawner.Spawn(levelData.ball, gravityDirection, basketSpawner.Basket.transform);
            SpawnRadioIfFirstTry(play, onComplete);
        }

        private void SpawnRadioIfFirstTry(Play play, Action onComplete)
        {
            if (play.Tries == 0)
                StartCoroutine(SpawnRadioRoutine(onComplete));
            else
                onComplete.Invoke();
        }

        private IEnumerator SpawnRadioRoutine(Action onComplete)
        {
            yield return new WaitUntil(HasFloorAtSpawnPoint);
            theRadioBehaviour.SetSpawnPointPosition();
            yield return new WaitForFixedUpdate();
            theRadioBehaviour.SetActive(true);
            onComplete.Invoke();
        }

        private bool HasFloorAtSpawnPoint()
        {
            if (floorMesh.EffectMeshObjects.Count == 0)
                return false;

            Vector3 origin = throwZone.WorldPosition + Vector3.up * 0.5f;
            return Physics.Raycast(origin, Vector3.down, 1f);
        }

        private Vector3 TrySpawnCourt(GameLevelData levelData)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                if (TrySpawnCourtOnce(levelData, out Vector3 gravityDirection))
                    return gravityDirection;

                FailAttempt();
            }

            return Vector3.zero;
        }

        private bool TrySpawnCourtOnce(GameLevelData levelData, out Vector3 gravityDirection)
        {
            throwZone.Spawn();
            gravityDirection = basketSpawner.SpawnAndGetGravityDirection(
                levelData.basket,
                levelData.gravity,
                throwZone.transform,
                levelData.distance.minMax);
            if (gravityDirection == Vector3.zero)
                return false;

            return TrySpawnObstacles(levelData) && TrySpawnModifiers(levelData) && FinishCourtAttempt();
        }

        private bool FinishCourtAttempt()
        {
            bool requiresBankShot =
                (obstacleSpawner != null && obstacleSpawner.RequiresBankShot()) ||
                (modifierSpawner != null && modifierSpawner.HasAnyObligatory());
            if (obstacleSpawner != null)
                obstacleSpawner.SetRequirementVisible(requiresBankShot);
            return true;
        }

        private bool TrySpawnObstacles(GameLevelData levelData)
        {
            if (obstacleSpawner == null)
                return true;

            return obstacleSpawner.TrySpawnAll(
                levelData.obstacles,
                throwZone.transform,
                basketSpawner.Basket.transform);
        }

        private bool TrySpawnModifiers(GameLevelData levelData)
        {
            if (modifierSpawner == null)
                return true;

            return modifierSpawner.TrySpawnAll(
                levelData.modifiers,
                throwZone.transform,
                basketSpawner.Basket.transform);
        }

        private void FailAttempt()
        {
            throwZone.Unspawn();
            basketSpawner.Hide();
            ClearLevelElements();
        }

        public void Clear()
        {
            ballSpawner.Unspawn();
            throwZone.Unspawn();
            basketSpawner.Hide();
            ClearLevelElements();
        }

        private void ClearLevelElements()
        {
            if (obstacleSpawner != null)
                obstacleSpawner.Clear();
            if (modifierSpawner != null)
                modifierSpawner.Clear();
        }
    }
}
