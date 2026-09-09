using System.Collections.Generic;
using Oculus.Interaction;
using UnityEngine;

namespace DigitalLove.Game.Balls
{
    [DefaultExecutionOrder(-100)]
    public class TwoHandGrabRequirement : MonoBehaviour, IGameObjectFilter
    {
        [SerializeField] private TouchHandGrabInteractable interactable;
        [SerializeField] private Grabbable grabbable;
        [SerializeField] private int minHoveringInteractors = 2;

        private void Awake()
        {
            interactable.MaxInteractors = minHoveringInteractors;
            interactable.MaxSelectingInteractors = minHoveringInteractors;
            grabbable.TransferOnSecondSelection = false;
            grabbable.MaxGrabPoints = minHoveringInteractors;
            EnsureTwoHandOnlyTransformer();
            interactable.InjectOptionalInteractorFilters(new List<IGameObjectFilter> { this });
        }

        public bool Filter(GameObject interactorObject)
        {
            if (CountSelecting() > 0)
                return true;

            return CountHovering() >= minHoveringInteractors;
        }

        private int CountHovering()
        {
            int count = 0;
            foreach (IInteractorView _ in interactable.InteractorViews)
                count++;
            return count;
        }

        private int CountSelecting()
        {
            int count = 0;
            foreach (IInteractorView _ in interactable.SelectingInteractorViews)
                count++;
            return count;
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
