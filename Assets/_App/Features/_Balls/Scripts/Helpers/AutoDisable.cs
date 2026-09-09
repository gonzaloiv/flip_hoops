using UnityEngine;

namespace DigitalLove.Game.Balls
{
    public class AutoDisable : MonoBehaviour
    {
        [SerializeField] private float secsBeforeDisable = 9;

        private float countdown;
        private bool hasBeenUnselected;

        private void OnEnable()
        {
            hasBeenUnselected = false;
        }

        public void OnUnselect()
        {
            hasBeenUnselected = true;
            countdown = secsBeforeDisable;
        }

        private void Update()
        {
            if (hasBeenUnselected)
            {
                countdown -= Time.deltaTime;
                if (countdown <= 0)
                    gameObject.SetActive(false);
            }
        }
    }
}