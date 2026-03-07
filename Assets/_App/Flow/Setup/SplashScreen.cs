using System;
using System.Collections;
using DigitalLove.Casual.Flow.Setup;
using DigitalLove.Global;
using DigitalLove.UI.Behaviours;
using UnityEngine;

namespace DigitalLove.App
{
    public class SplashScreen : BaseSplashScreenDirector
    {
        private const string SplashAnimationStateName = "Splash";

        [SerializeField] private SubtitlesLikeFollow splash;
        [SerializeField] private Animator animator;
        [SerializeField] private DebugBool skip;

        public void Hide()
        {
            animator.gameObject.SetActive(false);
        }

        public override void Play(Action onComplete)
        {
            IEnumerator SplashRoutine()
            {
                splash.ShowInCameraView();
                animator.gameObject.SetActive(true);
                animator.Play(SplashAnimationStateName);
                yield return null;
                AnimatorClipInfo clipInfo = animator.GetCurrentAnimatorClipInfo(0)[0];
                yield return new WaitForSeconds(clipInfo.clip.length);
                yield return new WaitForSeconds(0.33f);
                onComplete.Invoke();
            }
            if (!skip.Value)
            {
                StartCoroutine(SplashRoutine());
            }
            else
            {
                onComplete.Invoke();
            }
        }
    }
}