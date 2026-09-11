using DigitalLove.VFX;
using UnityEngine;

namespace DigitalLove.Game.Obstacles
{
    public class BankShotRejectFeedback : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private AudioClip rejectClip;
        [SerializeField] private ColorFade colorFade;

        public void PlayReject()
        {
            if (audioSource != null && rejectClip != null)
                audioSource.PlayOneShot(rejectClip);

            if (colorFade != null)
                colorFade.SetAndFadeBack();
        }
    }
}
