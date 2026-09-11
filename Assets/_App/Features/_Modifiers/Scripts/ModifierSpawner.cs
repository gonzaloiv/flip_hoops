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
        private ModifierPathCellPose pathPose;
        private ModifierRoomBoundaryPose roomPose;

        private ModifierPathCellPose PathPose =>
            pathPose ??= new ModifierPathCellPose(lateralHalfExtent, heightHalfExtent);

        private ModifierRoomBoundaryPose RoomPose =>
            roomPose ??= new ModifierRoomBoundaryPose(
                roomBoundaryHeightOffset,
                roomBoundaryMaxRay,
                clearanceRadius,
                occlusionMask);

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
                if (spawned[i] != null)
                {
                    spawned[i].Activated -= OnModifierActivated;
                    Destroy(spawned[i].gameObject);
                }
            }

            spawned.Clear();
            activationOrder.Clear();
        }

        public void ResetActivationsForThrow()
        {
            activationOrder.Clear();
            spawned.ResetAllForThrow();
        }

        public bool CanCreditMake()
        {
            if (!spawned.HasAnyObligatory())
                return true;

            return spawned.AllObligatorySatisfied();
        }

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

                ThrowScoreOpKind kind =
                    modifier.ScoreEffectKind == ModifierScoreEffectKind.FlatAdd
                        ? ThrowScoreOpKind.FlatAdd
                        : ThrowScoreOpKind.Multiply;
                into.Add(new ThrowScoreOp(kind, modifier.ScoreEffectValue));
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
            Vector3Int cell = ClampCell(placement.cell);
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

            return SpawnAt(placement, data, position, FaceBasket(position, basket));
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
            if (modifier == null || activationOrder.Contains(modifier))
                return;

            activationOrder.Add(modifier);
        }

        private static Quaternion FaceBasket(Vector3 position, Transform basket)
        {
            Vector3 look = basket.position - position;
            look.y = 0f;
            if (look.sqrMagnitude < 0.0001f)
                return Quaternion.identity;

            return Quaternion.LookRotation(look.normalized, Vector3.up);
        }

        private static Vector3Int ClampCell(Vector3Int cell)
        {
            return new Vector3Int(
                Mathf.Clamp(cell.x, -1, 1),
                Mathf.Clamp(cell.y, -1, 1),
                Mathf.Clamp(cell.z, -1, 1));
        }
    }
}
