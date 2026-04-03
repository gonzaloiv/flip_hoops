using System;
using System.Collections.Generic;
using System.Linq;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class BallsSpawner : MonoBehaviour
    {
        private const float SecsBeforeSpawn = 0.66f;

        [SerializeField] private BallSpawnPoint[] points;
        [SerializeField] private BallPrefabAmountPair[] pairs;

        private List<BallBehaviour> balls = new();
        private List<string> validIds;
        private Vector3 gravityDirection;

        public Action ballGrabbed = () => { };

        private void Awake()
        {
            foreach (BallPrefabAmountPair pair in pairs)
            {
                for (int i = 0; i < pair.amount; i++)
                {
                    BallBehaviour ball = Instantiate(pair.prefab, transform);
                    ball.SetActive(false);
                    balls.Add(ball);
                    ball.select.AddListener(OnBallSelected);
                    ball.unselect.AddListener(OnBallUnselected);
                }
            }
            void OnBallSelected() { ballGrabbed.Invoke(); }
        }

        private void OnBallUnselected()
        {
            if (gravityDirection == Vector3.zero)
                return;
            foreach (BallSpawnPoint point in points)
            {
                if (point.ball.HasBeenUnselected)
                    SetupBallForPoint(point, SecsBeforeSpawn);
            }
        }

        public void Spawn(List<BallData> balls, Vector3 gravityDirection)
        {
            Unspawn();
            validIds = balls.Select(b => b.id).ToList();
            this.gravityDirection = gravityDirection;
            foreach (BallSpawnPoint point in points)
            {
                SetupBallForPoint(point);
            }
        }

        private void SetupBallForPoint(BallSpawnPoint point, float secsBeforeSpawn = 0)
        {
            BallBehaviour ball = GetInactiveValidBall();
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

        private BallBehaviour GetInactiveValidBall()
        {
            BallBehaviour[] selection = balls.Where(b => !b.IsActive && validIds.Contains(b.Data.id)).ToArray();
            return selection[UnityEngine.Random.Range(0, selection.Length)];
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
    }

    [Serializable]
    public class BallSpawnPoint
    {
        public Transform reference;
        public BallBehaviour ball;
    }

    [Serializable]
    public class BallPrefabAmountPair
    {
        public BallBehaviour prefab;
        public int amount;
    }
}