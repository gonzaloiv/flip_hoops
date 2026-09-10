using UnityEngine;

namespace DigitalLove.Game.Obstacles
{
    public class BankShotRequirementPanel : MonoBehaviour
    {
        [SerializeField] private GameObject root;

        public void SetVisible(bool visible)
        {
            if (root != null)
                root.SetActive(visible);
            else
                gameObject.SetActive(visible);
        }

        public void Hide() => SetVisible(false);
    }
}
