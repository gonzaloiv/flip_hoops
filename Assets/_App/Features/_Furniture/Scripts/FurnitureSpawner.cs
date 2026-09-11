using System.Collections.Generic;
using DigitalLove.Game.BankShot;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Furniture
{
    public class FurnitureSpawner : MonoBehaviour
    {
        [SerializeField] private EffectMesh effectMesh;
        [SerializeField] private PhysicsMaterial bounceMaterial;
        [SerializeField] private Color boostTint = new Color(0.2f, 0.9f, 0.35f, 0.55f);
        [SerializeField] private Color reduceTint = new Color(0.95f, 0.25f, 0.2f, 0.55f);
        [SerializeField] private MRUKAnchor.SceneLabels suitableLabels =
            FurnitureRoomProbe.DefaultSuitableLabels;

        private readonly List<Collider> candidates = new();
        private readonly List<FurnitureZoneBehaviour> activeZones = new();
        private readonly List<FurnitureZoneBehaviour> activationOrder = new();
        private readonly FurnitureRoomProbe probe = new();

        public void TryRoll(FurnitureSeedData seed)
        {
            Clear();
            if (seed == null || seed.activatePercent <= 0.001f || effectMesh == null)
                return;

            probe.Collect(effectMesh, suitableLabels, candidates);
            if (candidates.Count == 0)
                return;

            ActivateFromSeed(seed);
        }

        public void Clear()
        {
            activationOrder.Clear();
            for (int i = 0; i < activeZones.Count; i++)
                TearDownZone(activeZones[i]);

            activeZones.Clear();
        }

        public void ResetActivationsForThrow()
        {
            activationOrder.Clear();
            for (int i = 0; i < activeZones.Count; i++)
            {
                if (activeZones[i] != null)
                    activeZones[i].ResetForThrow();
            }
        }

        public void CopyActivationOrderScoreOps(List<ThrowScoreOp> into)
        {
            if (into == null)
                return;

            for (int i = 0; i < activationOrder.Count; i++)
            {
                FurnitureZoneBehaviour zone = activationOrder[i];
                if (zone == null)
                    continue;

                into.Add(new ThrowScoreOp(zone.ScoreEffectKind, zone.ScoreEffectValue));
            }
        }

        private void ActivateFromSeed(FurnitureSeedData seed)
        {
            Shuffle(candidates);
            int activateCount = CountFromPercent(candidates.Count, seed.activatePercent);
            int boostCount = CountFromPercent(activateCount, seed.boostAmongActivesPercent);
            bool[] bounceFlags = BuildBounceFlags(
                activateCount,
                CountFromPercent(activateCount, seed.bounceAmongActivesPercent));

            for (int i = 0; i < activateCount; i++)
                CreateZone(candidates[i], seed, i < boostCount, bounceFlags[i]);
        }

        private static bool[] BuildBounceFlags(int activateCount, int bounceCount)
        {
            bool[] flags = new bool[activateCount];
            for (int i = 0; i < bounceCount; i++)
                flags[i] = true;

            for (int i = activateCount - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                bool tmp = flags[i];
                flags[i] = flags[j];
                flags[j] = tmp;
            }

            return flags;
        }

        private void CreateZone(
            Collider col,
            FurnitureSeedData seed,
            bool isBoost,
            bool withBounce)
        {
            if (col == null)
                return;

            FurnitureZoneBehaviour zone = col.gameObject.GetComponent<FurnitureZoneBehaviour>();
            if (zone == null)
                zone = col.gameObject.AddComponent<FurnitureZoneBehaviour>();

            ThrowScoreOpKind kind = isBoost ? seed.boostKind : seed.reduceKind;
            float value = isBoost ? seed.boostValue : seed.reduceValue;
            Color tint = isBoost ? boostTint : reduceTint;
            PhysicsMaterial bounce = withBounce ? bounceMaterial : null;
            zone.Configure(kind, value, tint, bounce);
            zone.Activated += OnZoneActivated;
            activeZones.Add(zone);
        }

        private void TearDownZone(FurnitureZoneBehaviour zone)
        {
            if (zone == null)
                return;

            zone.Activated -= OnZoneActivated;
            zone.Restore();
            Destroy(zone);
        }

        private void OnZoneActivated(FurnitureZoneBehaviour zone)
        {
            if (zone == null)
                return;

            activationOrder.Add(zone);
        }

        private static int CountFromPercent(int total, float percent)
        {
            if (total <= 0 || percent <= 0f)
                return 0;

            int count = Mathf.RoundToInt(total * Mathf.Clamp01(percent));
            if (count < 1)
                count = 1;

            return Mathf.Min(count, total);
        }

        private static void Shuffle(List<Collider> list)
        {
            for (int i = list.Count - 1; i > 0; i--)
            {
                int j = UnityEngine.Random.Range(0, i + 1);
                Collider tmp = list[i];
                list[i] = list[j];
                list[j] = tmp;
            }
        }
    }
}
