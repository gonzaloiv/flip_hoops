using DG.Tweening;
using UnityEngine;

namespace DigitalLove.Game
{
    public class ColorFade : MonoBehaviour
    {
        [SerializeField] private Renderer rend;
        [SerializeField] private float secs;
        [SerializeField] private Color color;

        private Color initialColor = Color.rosyBrown;

        public void Animate()
        {
            if (initialColor == Color.rosyBrown)
                initialColor = rend.material.color;
            rend.material.color = color;
            rend.material.DOColor(initialColor, secs);
        }
    }
}