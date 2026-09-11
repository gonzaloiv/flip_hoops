using System.Collections.Generic;
using DigitalLove.Game.BankShot;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class ModifierSpawner : MonoBehaviour
    {
        [SerializeField] private LayerMask occlusionMask;
        [SerializeField] private float lateralHalfExtent = 0.5f;
        [SerializeField] private float heightHalfExtent = 0.45f;
        [SerializeField] private float roomBoundaryHeightOffset = 0.9f;
        [SerializeField] private float roomBoundaryMaxRay = 4f;
        [SerializeField] private float clearanceRadius = 0.2f;

        private readonly List<ModifierBehaviour> spawned = new();
        private readonly List<ModifierBehaviour> activationOrder = new();
        private ThrowPathCellPose pathPose;
        private ModifierRoomBoundaryPose roomPose;

        private ThrowPathCellPose PathPose =>
            pathPose ??= new ThrowPathCellPose(lateralHalfExtent, heightHalfExtent);

        private ModifierRoomBoundaryPose RoomPose =>
            roomPose ??= new ModifierRoomBoundaryPose(
                roomBoundaryHeightOffset,
                roomBoundaryMaxRay,
                clearanceRadius,
                occlusionMask,
                lateralHalfExtent,
                heightHalfExtent);

        public IReadOnlyList<ModifierBehaviour> Spawned => spawned;

        public bool TrySpawnAll(
            ModifierPlacement[] placements,
            Transform throwZone,
            Transform basket)
        {
            Clear();
            if (placements == null || placements.Length == 0)
                return true;

            for (int i = 0; i < placements.Length; i++)
            {
                if (!TrySpawnOne(placements[i], throwZone, basket))
                {
                    Clear();
                    return false;
                }
            }

            return true;
        }

        public void Clear()
        {
            for (int i = 0; i < spawned.Count; i++)
            {
                if (spawned[i] == null)
                    continue;

                spawned[i].Activated -= OnModifierActivated;
                Destroy(spawned[i].gameObject);
            }

            spawned.Clear();
            activationOrder.Clear();
        }

        public void ResetActivationsForThrow()
        {
            activationOrder.Clear();
            spawned.ResetAllForThrow();
        }

        public bool CanCreditMake() => spawned.CanCreditMake();

        public bool HasAnyObligatory() => spawned.HasAnyObligatory();

        public void CopyActivationOrderScoreOps(List<ThrowScoreOp> into)
        {
            if (into == null)
                return;

            for (int i = 0; i < activationOrder.Count; i++)
            {
                ModifierBehaviour modifier = activationOrder[i];
                if (modifier == null)
                    continue;

                into.Add(new ThrowScoreOp(modifier.ScoreEffectKind, modifier.ScoreEffectValue));
            }
        }

        private bool TrySpawnOne(
            ModifierPlacement placement,
            Transform throwZone,
            Transform basket)
        {
            if (placement == null || placement.modifier == null || placement.modifier.prefab == null)
                return false;

            ModifierData data = placement.modifier;
            Vector3Int cell = ThrowPathCellPose.ClampCell(placement.cell);
            if (data.placementMode == ModifierPlacementMode.RoomBoundary)
                return TrySpawnRoomBoundary(placement, data, throwZone, basket, cell);

            if (TrySpawnPathCell(placement, data, throwZone, basket, cell))
                return true;

            Vector3Int mirror = PathPose.MirrorLateral(cell);
            if (mirror == cell)
                return false;

            return TrySpawnPathCell(placement, data, throwZone, basket, mirror);
        }

        private bool TrySpawnPathCell(
            ModifierPlacement placement,
            ModifierData data,
            Transform throwZone,
            Transform basket,
            Vector3Int cell)
        {
            Vector3 position = PathPose.WorldPosition(throwZone, basket, cell);
            if (Physics.CheckSphere(position, clearanceRadius, occlusionMask))
                return false;

            return SpawnAt(
                placement,
                data,
                position,
                ThrowPathCellPose.FaceBasket(position, basket));
        }

        private bool TrySpawnRoomBoundary(
            ModifierPlacement placement,
            ModifierData data,
            Transform throwZone,
            Transform basket,
            Vector3Int cell)
        {
            if (!RoomPose.TryResolve(throwZone, basket, cell, out Vector3 position, out Quaternion rotation))
                return false;

            return SpawnAt(placement, data, position, rotation);
        }

        private bool SpawnAt(
            ModifierPlacement placement,
            ModifierData data,
            Vector3 position,
            Quaternion rotation)
        {
            ModifierBehaviour instance = Instantiate(
                data.prefab,
                position,
                rotation,
                transform);
            instance.Configure(placement.obligatory, data.scoreEffectKind, data.scoreEffectValue);
            instance.Activated += OnModifierActivated;
            spawned.Add(instance);
            return true;
        }

        private void OnModifierActivated(ModifierBehaviour modifier)
        {
            if (modifier == null)
                return;

            activationOrder.Add(modifier);
        }
    }
}
