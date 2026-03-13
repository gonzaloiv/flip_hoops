using System.Collections;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Ball
{
    public class AutoDisable : MonoBehaviour
    {
        [SerializeField] private float secsBeforeDisable = 5;

        private Coroutine coroutine;

        public void OnUnselect()
        {
            IEnumerator AutoDisableRoutine()
            {
                yield return new WaitForSecondsRealtime(secsBeforeDisable);
                this.SetActive(false);
            }
            coroutine = StartCoroutine(AutoDisableRoutine());
        }

        private void OnDisable()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }
    }
}