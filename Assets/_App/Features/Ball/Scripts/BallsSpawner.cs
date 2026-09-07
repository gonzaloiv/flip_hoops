using System;
using System.Collections.Generic;
using DigitalLove.Global;
using UnityEngine;
using UnityEngine.Pool;

namespace DigitalLove.Game.Balls
{
    public class BallsSpawner : MonoBehaviour
    {
        private const float SecsBeforeSpawn = 0.66f;
        private const int MaxBallsPerType = 10;

        [SerializeField] private BallSpawnPoint[] points;

        private readonly List<BallBehaviour> rented = new();
        private ObjectPool<BallBehaviour> pool;
        private BallData currentData;
        private Vector3 gravityDirection;
        private bool isReleasing;

        public BallBehaviour ValidBall => rented.FindValid();

        public Action ballGrabbed = () => { };

        public void Invoke_BallGrabbed() => ballGrabbed.Invoke();

        public void Spawn(BallData ball, Vector3 gravityDirection)
        {
            Unspawn();
            EnsurePool(ball);
            this.gravityDirection = gravityDirection;
            foreach (BallSpawnPoint point in points)
                SetupBallForPoint(point);
        }

        public void Unspawn()
        {
            gravityDirection = Vector3.zero;
            ClearSpawnPoints();
            ReleaseAllRented();
        }

        public void SetIsInStreak(bool isInStreak)
        {
            for (int i = 0; i < rented.Count; i++)
                rented[i].SetIsInStreak(isInStreak);
        }

        private void EnsurePool(BallData data)
        {
            if (pool != null && currentData == data)
                return;

            ClearPool();
            currentData = data;
            pool = ComponentObjectPool.Create(data.prefab, transform, OnBallCreated, MaxBallsPerType);
            ComponentObjectPool.Warm(pool, MaxBallsPerType);
        }

        private void OnBallCreated(BallBehaviour ball)
        {
            ball.select.AddListener(OnBallSelected);
            ball.unselect.AddListener(OnBallUnselected);
            ball.becameInactive.AddListener(() => OnBallBecameInactive(ball));
        }

        private void OnBallSelected() => ballGrabbed.Invoke();

        private void OnBallUnselected()
        {
            foreach (BallSpawnPoint point in points)
            {
                if (point.ball != null && point.ball.HasBeenUnselected)
                    SetupBallForPoint(point, SecsBeforeSpawn);
            }
        }

        private void SetupBallForPoint(BallSpawnPoint point, float secsBeforeSpawn = 0)
        {
            BallBehaviour ball = Rent();
            ball.transform.position = point.reference.position;
            ball.GravityDirection = gravityDirection;
            point.ball = ball;
            if (secsBeforeSpawn != 0)
                this.InvokeAfterSecs(secsBeforeSpawn, () => ActivateIfRented(ball));
            else
                ball.SetActive(true);
        }

        private void ActivateIfRented(BallBehaviour ball)
        {
            if (rented.Contains(ball))
                ball.SetActive(true);
        }

        private BallBehaviour Rent()
        {
            BallBehaviour ball = pool.Get();
            rented.Add(ball);
            return ball;
        }

        private void OnBallBecameInactive(BallBehaviour ball)
        {
            ClearSpawnPoint(ball);
            Release(ball);
        }

        private void Release(BallBehaviour ball)
        {
            if (isReleasing || ball == null || !rented.Remove(ball))
                return;

            isReleasing = true;
            pool.Release(ball);
            isReleasing = false;
        }

        private void ReleaseAllRented()
        {
            for (int i = rented.Count - 1; i >= 0; i--)
                Release(rented[i]);
        }

        private void ClearSpawnPoints()
        {
            foreach (BallSpawnPoint point in points)
                point.ball = null;
        }

        private void ClearSpawnPoint(BallBehaviour ball)
        {
            foreach (BallSpawnPoint point in points)
            {
                if (point.ball == ball)
                    point.ball = null;
            }
        }

        private void ClearPool()
        {
            ReleaseAllRented();
            pool?.Clear();
            pool = null;
            currentData = null;
        }

        private void OnDestroy()
        {
            ClearPool();
        }
    }

    [Serializable]
    public class BallSpawnPoint
    {
        public Transform reference;
        public BallBehaviour ball;
    }
}
