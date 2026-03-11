using System;
using System.Collections.Generic;
using DigitalLove.Game.Court;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Ball
{
    public class BallSpawner : MonoBehaviour
    {
        private const float SecsBeforeSpawn = 0.66f;

        [SerializeField] private BallSpawnPoint[] points;
        [SerializeField] private BallBehaviour prefab;

        private List<BallBehaviour> balls = new();
        private GravityData gravity;

        private bool IsSpawning => gravity != null;

        public void Spawn(GravityData gravity)
        {
            this.gravity = gravity;
            foreach (BallSpawnPoint point in points)
            {
                InstantiateBallForPoint(point);
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
                    InstantiateBallForPoint(point, SecsBeforeSpawn);
            }
        }

        private void InstantiateBallForPoint(BallSpawnPoint point, float secsBeforeSpawn = 0)
        {
            BallBehaviour ball = Instantiate(prefab, transform);
            ball.transform.position = point.reference.position;
            ball.Gravity = gravity;
            balls.Add(ball);
            point.ball = ball;
            ball.SetActive(false);
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