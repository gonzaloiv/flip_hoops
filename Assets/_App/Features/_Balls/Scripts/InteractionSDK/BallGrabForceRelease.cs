using Oculus.Interaction;

namespace DigitalLove.Game.Balls
{
    /// <summary>
    /// Forces Meta grab interactors to unselect when sampled hand speed shows throw intent,
    /// so TouchHandGrab finger-lock / collider gating cannot keep the ball stuck to the hand.
    /// </summary>
    public class BallGrabForceRelease
    {
        private readonly TouchHandGrabInteractable touchHandGrab;
        private readonly GrabInteractable grabInteractable;

        public BallGrabForceRelease(
            TouchHandGrabInteractable touchHandGrab,
            GrabInteractable grabInteractable)
        {
            this.touchHandGrab = touchHandGrab;
            this.grabInteractable = grabInteractable;
        }

        public bool TryRelease(float sampledSpeed, float speedThreshold)
        {
            if (sampledSpeed < speedThreshold)
                return false;

            ForceTouchHandRelease();
            ForceControllerGrabRelease();
            return true;
        }

        private void ForceTouchHandRelease()
        {
            if (touchHandGrab == null)
                return;

            foreach (TouchHandGrabInteractor interactor in touchHandGrab.SelectingInteractors)
                interactor.SetComputeShouldUnselectOverride(() => true);
        }

        private void ForceControllerGrabRelease()
        {
            if (grabInteractable == null)
                return;

            foreach (GrabInteractor interactor in grabInteractable.SelectingInteractors)
                interactor.ForceRelease();
        }
    }
}
