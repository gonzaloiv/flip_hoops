using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Game.Court;
using DigitalLove.Game.Levels;
using DigitalLove.Game.UI;
using UnityEngine;

namespace DigitalLove.Game
{
    public class CourtSetupHelper : MonoBehaviour
    {
        private const int MaxAttempts = 5;

        [SerializeField] private GravitySelector gravitySelector;
        [SerializeField] private BallsSpawner ballSpawner;
        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private ThrowZone throwZone;
        [SerializeField] private PosterBehaviour[] posters;

        public float DistanceToCamera =>
            Vector3.Distance(basketSpawner.Basket.WorldPosition, Camera.main.transform.position);

        public void Spawn(GameLevelData levelData)
        {
            GravityData gravity = gravitySelector.SelectRandom(levelData.gravities);
            Vector3 gravityDirection = TrySpawnBasket(gravity, levelData.distance);
            posters.Spawn(gravityDirection);
            throwZone.SetReference(basketSpawner.Basket.transform);
            ballSpawner.Spawn(levelData.balls, gravityDirection);
        }

        public void Clear()
        {
            ballSpawner.Unspawn();
            throwZone.Unspawn();
            basketSpawner.Hide();
        }

        private Vector3 TrySpawnBasket(GravityData gravity, float[] distances)
        {
            for (int attempt = 0; attempt < MaxAttempts; attempt++)
            {
                throwZone.Spawn();
                Vector3 gravityDirection = basketSpawner.SpawnAndGetGravityDirection(gravity, throwZone.transform, distances);
                if (gravityDirection != Vector3.zero)
                    return gravityDirection;

                throwZone.Unspawn();
                basketSpawner.Hide();
            }

            return Vector3.zero;
        }
    }
}
