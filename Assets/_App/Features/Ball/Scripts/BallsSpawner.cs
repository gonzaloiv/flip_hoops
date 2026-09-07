using System;
using System.Collections.Generic;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class BallsSpawner : MonoBehaviour
    {
        private const float SecsBeforeSpawn = 0.66f;
        private const int MaxBallsPerType = 10;

        [SerializeField] private BallSpawnPoint[] points;
        [SerializeField] private BallData[] ballsData;

        private List<BallBehaviour> balls = new();
        private string currentBallId;
        private Vector3 gravityDirection;

        public BallBehaviour ValidBall => balls.FindValid();

        public Action ballGrabbed = () => { };

        private void Awake() // TODO: Pooling the right balls for each level
        {
            foreach (BallData data in ballsData)
            {
                for (int i = 0; i < MaxBallsPerType; i++)
                {
                    BallBehaviour ball = Instantiate(data.prefab, transform);
                    ball.SetActive(false);
                    balls.Add(ball);
                    ball.select.AddListener(OnBallSelected);
                    ball.unselect.AddListener(OnBallUnselected);
                }
            }
            void OnBallSelected() { ballGrabbed.Invoke(); }
        }

        public void Invoke_BallGrabbed() => ballGrabbed.Invoke();

        private void OnBallUnselected()
        {
            foreach (BallSpawnPoint point in points)
            {
                if (point.ball.HasBeenUnselected)
                    SetupBallForPoint(point, SecsBeforeSpawn);
            }
        }

        public void Spawn(BallData ball, Vector3 gravityDirection)
        {
            Unspawn();
            currentBallId = ball.id;
            this.gravityDirection = gravityDirection;
            foreach (BallSpawnPoint point in points)
            {
                SetupBallForPoint(point);
            }
        }

        private void SetupBallForPoint(BallSpawnPoint point, float secsBeforeSpawn = 0)
        {
            BallBehaviour ball = balls.PickRandomInactiveWithId(new List<string> { currentBallId });
            ball.transform.position = point.reference.position;
            ball.GravityDirection = gravityDirection;
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
            gravityDirection = Vector3.zero;
            foreach (BallBehaviour ball in balls)
            {
                ball.transform.position = new Vector3(100, 100, 100);
                ball.SetActive(false);
            }
            foreach (BallSpawnPoint point in points)
            {
                if (point.ball != null)
                    point.ball = null;
            }
        }

        public void SetIsInStreak(bool isInStreak)
        {
            foreach (BallBehaviour ball in balls)
            {
                ball.SetIsInStreak(isInStreak);
            }
        }
    }

    [Serializable]
    public class BallSpawnPoint
    {
        public Transform reference;
        public BallBehaviour ball;
    }

    [Serializable]
    public class BallPrefabDataPair
    {
        public BallData data;
        public BallBehaviour prefab;
    }
}