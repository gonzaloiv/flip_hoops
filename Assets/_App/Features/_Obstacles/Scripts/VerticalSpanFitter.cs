using UnityEngine;

namespace DigitalLove.Global
{
    public class VerticalSpanFitter : MonoBehaviour
    {
        [SerializeField] private CapsuleCollider capsule;
        [SerializeField] private Transform visual;
        [SerializeField] private float minHeight = 0.4f;
        [SerializeField] private float inset = 0.02f;
        [SerializeField] private float maxRayDistance = 4f;
        [SerializeField] private float visualHeightAtUnitScale = 2f;

        public bool TryFit(LayerMask surfaceMask)
        {
            Vector3 seed = transform.position;
            if (!TryFindSupport(seed, surfaceMask, out float supportY))
                return false;

            if (!TryFindCeiling(seed.x, supportY, seed.z, surfaceMask, out float ceilingY))
                return false;

            return TryApplySpan(seed.x, seed.z, supportY, ceilingY);
        }

        private bool TryFindSupport(Vector3 seed, LayerMask surfaceMask, out float supportY)
        {
            Vector3 origin = new Vector3(seed.x, seed.y + maxRayDistance * 0.5f, seed.z);
            if (!Physics.Raycast(origin, Vector3.down, out RaycastHit hit, maxRayDistance, surfaceMask))
            {
                supportY = 0f;
                return false;
            }

            supportY = hit.point.y;
            return true;
        }

        private bool TryFindCeiling(
            float x,
            float supportY,
            float z,
            LayerMask surfaceMask,
            out float ceilingY)
        {
            Vector3 origin = new Vector3(x, supportY + inset + 0.05f, z);
            if (!Physics.Raycast(origin, Vector3.up, out RaycastHit hit, maxRayDistance, surfaceMask))
            {
                ceilingY = 0f;
                return false;
            }

            ceilingY = hit.point.y;
            return true;
        }

        private bool TryApplySpan(float x, float z, float supportY, float ceilingY)
        {
            float bottom = supportY + inset;
            float top = ceilingY - inset;
            float height = top - bottom;
            if (height < minHeight)
                return false;

            ApplyHeight(x, z, bottom, height);
            return true;
        }

        private void ApplyHeight(float x, float z, float bottom, float height)
        {
            transform.position = new Vector3(x, bottom + height * 0.5f, z);
            CapsuleCollider col = capsule != null ? capsule : GetComponent<CapsuleCollider>();
            if (col != null)
            {
                col.height = height;
                col.center = Vector3.zero;
            }

            ScaleVisual(height);
        }

        private void ScaleVisual(float height)
        {
            if (visual == null)
                return;

            Vector3 scale = visual.localScale;
            float unitHeight = visualHeightAtUnitScale > 0.01f ? visualHeightAtUnitScale : 2f;
            scale.y = height / unitHeight;
            visual.localScale = scale;
            visual.localPosition = Vector3.zero;
        }
    }
}
