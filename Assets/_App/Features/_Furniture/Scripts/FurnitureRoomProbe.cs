using System.Collections.Generic;
using Meta.XR.MRUtilityKit;
using UnityEngine;
using static Meta.XR.MRUtilityKit.EffectMesh;

namespace DigitalLove.Game.Furniture
{
    public class FurnitureRoomProbe
    {
        public static readonly MRUKAnchor.SceneLabels DefaultSuitableLabels =
            MRUKAnchor.SceneLabels.TABLE
            | MRUKAnchor.SceneLabels.COUCH
            | MRUKAnchor.SceneLabels.BED
            | MRUKAnchor.SceneLabels.STORAGE;

        public void Collect(
            EffectMesh effectMesh,
            MRUKAnchor.SceneLabels suitableLabels,
            List<Collider> into)
        {
            into.Clear();
            if (effectMesh == null || effectMesh.EffectMeshObjects == null)
                return;

            foreach (KeyValuePair<MRUKAnchor, EffectMeshObject> pair in effectMesh.EffectMeshObjects)
                TryAddCandidate(pair.Key, pair.Value, suitableLabels, into);
        }

        private static void TryAddCandidate(
            MRUKAnchor anchor,
            EffectMeshObject meshObject,
            MRUKAnchor.SceneLabels suitableLabels,
            List<Collider> into)
        {
            if (anchor == null || meshObject.effectMeshGO == null)
                return;
            if (!anchor.HasAnyLabel(suitableLabels))
                return;

            Collider col = meshObject.collider != null
                ? meshObject.collider
                : meshObject.effectMeshGO.GetComponent<Collider>();
            if (col == null || into.Contains(col))
                return;

            into.Add(col);
        }
    }
}
