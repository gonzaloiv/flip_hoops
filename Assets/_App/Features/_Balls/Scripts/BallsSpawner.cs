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
        private Transform throwTarget;
        private bool isReleasing;

        public BallBehaviour ValidBall => rented.FindValid();

        public Action ballGrabbed = () => { };
        public Action ballThrown = () => { };

        public void Invoke_BallGrabbed() => ballGrabbed.Invoke();

        public void Spawn(BallData ball, Vector3 gravityDirection, Transform throwTarget = null)
        {
            Unspawn();
            EnsurePool(ball);
            this.gravityDirection = gravityDirection;
            this.throwTarget = throwTarget;
            foreach (BallSpawnPoint point in points)
                SetupBallForPoint(point);
        }

        public void Unspawn()
        {
            gravityDirection = Vector3.zero;
            throwTarget = null;
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
            ballThrown.Invoke();
            foreach (BallSpawnPoint point in points)
            {
                if (NeedsRefill(point))
                    SetupBallForPoint(point, SecsBeforeSpawn);
            }
        }

        private static bool NeedsRefill(BallSpawnPoint point)
        {
            if (point.refillPending)
                return false;
            if (point.ball == null)
                return true;
            return point.ball.HasBeenUnselected;
        }

        private void SetupBallForPoint(BallSpawnPoint point, float secsBeforeSpawn = 0)
        {
            BallBehaviour previous = point.ball;
            BallBehaviour ball = Rent();
            ball.transform.position = point.reference.position;
            ball.GravityDirection = gravityDirection;
            ball.SetThrowTarget(throwTarget);
            point.ball = ball;
            point.refillPending = secsBeforeSpawn != 0;
            ScheduleOrActivate(point, ball, secsBeforeSpawn);
            ReleaseOrphan(previous, ball);
        }

        private void ScheduleOrActivate(BallSpawnPoint point, BallBehaviour ball, float secsBeforeSpawn)
        {
            if (secsBeforeSpawn != 0)
                this.InvokeAfterSecs(secsBeforeSpawn, () => ActivatePending(point, ball));
            else
                ball.SetActive(true);
        }

        private void ActivatePending(BallSpawnPoint point, BallBehaviour ball)
        {
            if (point.ball != ball || !rented.Contains(ball))
            {
                if (point.ball == ball)
                    point.refillPending = false;
                return;
            }

            point.refillPending = false;
            ball.SetActive(true);
        }

        private void ReleaseOrphan(BallBehaviour previous, BallBehaviour next)
        {
            if (previous != null && previous != next && !previous.IsActive)
                Release(previous);
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
            {
                point.ball = null;
                point.refillPending = false;
            }
        }

        private void ClearSpawnPoint(BallBehaviour ball)
        {
            foreach (BallSpawnPoint point in points)
            {
                if (point.ball != ball)
                    continue;
                point.ball = null;
                point.refillPending = false;
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
        [NonSerialized] public bool refillPending;
    }
}
