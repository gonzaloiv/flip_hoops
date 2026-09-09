using Oculus.Interaction;
using UnityEngine;

namespace DigitalLove.Game.Balls
{
    [DefaultExecutionOrder(-100)]
    public class TwoHandGrabRequirement : MonoBehaviour
    {
        [SerializeField] private TouchHandGrabInteractable interactable;
        [SerializeField] private Grabbable grabbable;
        [SerializeField] private int maxSelectingInteractors = 2;

        private void Awake()
        {
            interactable.MaxInteractors = -1;
            interactable.MaxSelectingInteractors = maxSelectingInteractors;
            grabbable.TransferOnSecondSelection = false;
            grabbable.MaxGrabPoints = maxSelectingInteractors;
            EnsureTwoHandOnlyTransformer();
        }

        private void EnsureTwoHandOnlyTransformer()
        {
            GrabFreeTransformer transformer = GetComponent<GrabFreeTransformer>();
            if (transformer == null)
                transformer = gameObject.AddComponent<GrabFreeTransformer>();

            grabbable.InjectOptionalTwoGrabTransformer(transformer);
        }
    }
}
