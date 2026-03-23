using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using DigitalLove.Global;
using DigitalLove.Localization;
using DigitalLove.UI.Behaviours;
using Reflex.Attributes;
using TMPro;
using UnityEngine;

namespace DigitalLove.Game.UI
{
    public class FindTheHoopPanel : MonoBehaviour
    {
        [SerializeField] private float secsBeforeHiding = 4f;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private SubtitlesLikeFollow subtitlesLikeFollow;

        [Inject] private MemoryDataClient memoryDataClient;

        public void Show()
        {
            if (memoryDataClient.Get<Play>().Tries < 1)
            {
                gameObject.SetActive(true);
                label.text = LocalizationUtil.GetValue("find_the_hoop");
                subtitlesLikeFollow.ShowInCameraView();
                this.InvokeAfterSecs(secsBeforeHiding, () => gameObject.SetActive(false));
            }
            else
            {
                Hide();
            }
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}