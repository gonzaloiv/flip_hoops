using DigitalLove.Game.Balls;
using DigitalLove.Game.Basket;
using DigitalLove.Localization;
using UnityEngine;

namespace DigitalLove.Game
{
    public class StreakCounter : MonoBehaviour
    {
        private const float StreakMultiplierIncreaseRate = 0.5f;
        private const int MinStreakBasketCount = 2;
        private const float SecsToBreakStreak = 2;

        [SerializeField] private BasketSpawner basketSpawner;
        [SerializeField] private BallsSpawner ballsSpawner;

        private int streak;
        private float countdown;

        public float CurrentStreakMultiplier => IsInStreak ? 1 + (streak - MinStreakBasketCount) * StreakMultiplierIncreaseRate : 1;
        public bool IsInStreak => streak > MinStreakBasketCount;

        public void Reset()
        {
            streak = 0;
        }

        public void IncrementStreak()
        {
            streak++;
            countdown = SecsToBreakStreak;
            ballsSpawner.SetIsInStreak(IsInStreak);
            if (IsInStreak && streak % 2 != 0)
                basketSpawner.Panel.ShowShort(LocalizationUtil.GetValue("on_fire"));
        }

        private void Update()
        {
            if (streak == 0)
                return;
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                ballsSpawner.SetIsInStreak(false);
                streak = 0;
            }
        }
    }
}