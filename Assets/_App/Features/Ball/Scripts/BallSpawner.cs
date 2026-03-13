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

        public Action ballGrabbed = () => { };

        private void Awake()
        {
            for (int i = 0; i < TotalBalls; i++)
            {
                BallBehaviour ball = Instantiate(prefab, transform);
                ball.SetActive(false);
                balls.Add(ball);
                ball.select.AddListener(OnBallSelected);
                ball.unselect.AddListener(OnBallUnselected);
            }
            void OnBallSelected() { ballGrabbed.Invoke(); }
        }

        private void OnBallUnselected()
        {
            foreach (BallSpawnPoint point in points)
            {
                if (point.ball.HasBeenUnselected)
                    SetupBallForPoint(point, SecsBeforeSpawn);
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

        public void Unspawn()
        {
            gravity = null;
            foreach (BallBehaviour ball in balls)
            {
                ball.SetActive(false);
            }
            foreach (BallSpawnPoint point in points)
            {
                point.ball = null;
            }
        }
    }

    [Serializable]
    public class BallSpawnPoint
    {
        public Transform reference;
        public BallBehaviour ball;
    }
}