using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLove.Game.Court;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Ball
{
    public class BallSpawner : MonoBehaviour
    {
        private const float SecsBeforeSpawn = 0.66f;
        private const float TotalBalls = 25;

        [SerializeField] private BallSpawnPoint[] points;
        [SerializeField] private BallBehaviour prefab;

        private List<BallBehaviour> balls = new();
        private GravityData gravity;

        private bool IsSpawning => gravity != null;

        private void Awake()
        {
            for (int i = 0; i < TotalBalls; i++)
            {
                BallBehaviour ball = Instantiate(prefab, transform);
                ball.SetActive(false);
                balls.Add(ball);
            }
        }

        public void Spawn(GravityData gravity)
        {
            this.gravity = gravity;
            foreach (BallSpawnPoint point in points)
            {
                SetupBallForPoint(point);
            }
        }

        public void Unspawn()
        {
            gravity = null;
            foreach (BallBehaviour ball in balls)
            {
                if (ball != null)
                    Destroy(ball.gameObject);
            }
            balls.Clear();
        }

        private void Update()
        {
            if (!IsSpawning)
                return;
            foreach (BallSpawnPoint point in points)
            {
                if (!point.HasGrabbableBall)
                    SetupBallForPoint(point, SecsBeforeSpawn);
            }
        }

        private void SetupBallForPoint(BallSpawnPoint point, float secsBeforeSpawn = 0)
        {
            BallBehaviour ball = balls.FirstOrDefault(b => !b.IsActive);
            ball.transform.position = point.reference.position;
            ball.Gravity = gravity;
            point.ball = ball;
            if (secsBeforeSpawn != 0)
            {
                this.InvokeAfterSecs(secsBeforeSpawn, () => ball.SetActive(true));
            }
            else
            {
                ball.SetActive(true);
            }
        }
    }

    [Serializable]
    public class BallSpawnPoint
    {
        public Transform reference;
        public BallBehaviour ball;

        public bool HasGrabbableBall => ball != null && !ball.HasBeenUnselected;
    }
}