using DigitalLove.Global;
using TMPro;
using UnityEngine;

namespace DigitalLove.Game.Basket
{
    public class BasketPanel : MonoBehaviour
    {
        [SerializeField] private string tableName;
        [SerializeField] private TextMeshProUGUI initLabel;
        [SerializeField] private TextMeshProUGUI infoLabel;
        [SerializeField] private float secsActive = 5;

        public void Show(string initText, string infoText)
        {
            gameObject.SetActive(true);
            initLabel.text = initText;
            initLabel.text = infoText;
            this.InvokeAfterSecs(secsActive, Hide);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}
