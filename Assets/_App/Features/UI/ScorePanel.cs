using TMPro;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class ScorePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;

        public void Show(int score)
        {
            gameObject.SetActive(true);
            label.text = score.ToString();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
