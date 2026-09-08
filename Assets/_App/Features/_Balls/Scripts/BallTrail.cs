using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class BallTrail : MonoBehaviour
    {
        [SerializeField] private TrailRenderer trail;
        [SerializeField] private ParticleSystem ps;
        [SerializeField] private ColorValue defaultColor;
        [SerializeField] private ColorValue inStreakColor;

        public void Reset()
        {
            trail.Clear();
            ShowStreak(false);
        }

        public void ShowStreak(bool isInStreak)
        {
            if (isInStreak)
                ps.Play();
            Color color = isInStreak ? inStreakColor.value : defaultColor.value;
            color.a = trail.startColor.a;
            trail.startColor = color;
        }
    }
}