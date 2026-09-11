using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using DigitalLove.Game.Furniture;
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
        [SerializeField] private FurnitureSpawner furnitureSpawner;
        [SerializeField] private ThrowZone throwZone;
        [SerializeField] private PosterBehaviour[] posters;
        [SerializeField] private TheRadioBehaviour theRadioBehaviour;
        [SerializeField] private EffectMesh floorMesh;
        [SerializeField] private float throwAxisMaxRay = 8f;

        public float DistanceToCamera
        {
            get
            {
                if (basketSpawner.Basket == null || Camera.main == null)
                    return 0f;
                return Vector3.Distance(basketSpawner.Basket.WorldPosition, Camera.main.transform.position);
            }
        }

        public void Init() => theRadioBehaviour.SetActive(false);

        public void Spawn(GameLevelData levelData, Play play, Action<bool> onComplete)
        {
            Vector3 gravityDirection = TrySpawnCourt(levelData);
            if (gravityDirection == Vector3.zero || basketSpawner.Basket == null)
            {
                onComplete?.Invoke(false);
                return;
            }

            posters.Spawn(gravityDirection);
            throwZone.SetReference(basketSpawner.Basket.transform);
            ballSpawner.Spawn(levelData.ball, gravityDirection, basketSpawner.Basket.transform);
            SpawnRadioIfFirstTry(play, () => onComplete?.Invoke(true));
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
            gravityDirection = Vector3.zero;
            if (!throwZone.TrySpawnForBand(
                    levelData.distance,
                    throwAxisMaxRay,
                    out float scaleFactor,
                    out float desiredMeters))
                return false;

            gravityDirection = basketSpawner.SpawnOnAxis(
                levelData.basket,
                levelData.gravity,
                throwZone.transform,
                desiredMeters,
                scaleFactor);
            if (gravityDirection == Vector3.zero)
                return false;

            SoftSpawnProps(levelData, scaleFactor);
            return FinishCourtAttempt(levelData);
        }

        private void SoftSpawnProps(GameLevelData levelData, float scaleFactor)
        {
            TrySpawnObstacles(levelData);
            TrySpawnModifiers(levelData, scaleFactor);
        }

        private bool FinishCourtAttempt(GameLevelData levelData)
        {
            bool requiresBankShot =
                (obstacleSpawner != null && obstacleSpawner.RequiresBankShot()) ||
                (modifierSpawner != null && modifierSpawner.HasAnyObligatory());
            if (obstacleSpawner != null)
                obstacleSpawner.SetRequirementVisible(requiresBankShot);

            TryRollFurniture(levelData);
            return true;
        }

        private void TryRollFurniture(GameLevelData levelData)
        {
            if (furnitureSpawner == null)
                return;

            furnitureSpawner.TryRoll(levelData.furnitureSeed);
        }

        private void TrySpawnObstacles(GameLevelData levelData)
        {
            if (obstacleSpawner == null)
                return;

            obstacleSpawner.TrySpawnAll(
                levelData.obstacles,
                throwZone.transform,
                basketSpawner.Basket.transform);
        }

        private void TrySpawnModifiers(GameLevelData levelData, float scaleFactor)
        {
            if (modifierSpawner == null)
                return;

            modifierSpawner.TrySpawnAll(
                levelData.modifiers,
                throwZone.transform,
                basketSpawner.Basket.transform,
                scaleFactor);
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
            if (furnitureSpawner != null)
                furnitureSpawner.Clear();
        }
    }
}
