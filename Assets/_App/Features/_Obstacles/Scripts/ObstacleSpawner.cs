using System.Collections.Generic;
using UnityEngine;

namespace DigitalLove.Game.Obstacles
{
    public class ObstacleSpawner : MonoBehaviour
    {
        [SerializeField] private LayerMask occlusionMask;
        [SerializeField] private float lateralHalfExtent = 0.5f;
        [SerializeField] private float heightHalfExtent = 0.45f;
        [SerializeField] private BankShotRequirementPanel requirementPanel;

        private readonly List<ObstacleBehaviour> spawned = new();
        private ObstacleGridPose gridPose;

        private ObstacleGridPose GridPose =>
            gridPose ??= new ObstacleGridPose(lateralHalfExtent, heightHalfExtent);

        public IReadOnlyList<ObstacleBehaviour> Spawned => spawned;

        public bool TrySpawnAll(
            ObstaclePlacement[] placements,
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

        public void SetRequirementVisible(bool visible) =>
            requirementPanel?.SetVisible(visible);

        public void Clear()
        {
            for (int i = 0; i < spawned.Count; i++)
            {
                if (spawned[i] != null)
                    Destroy(spawned[i].gameObject);
            }

            spawned.Clear();
            requirementPanel?.Hide();
        }

        public void ResetHitsForThrow() => spawned.ResetAllForThrow();

        public bool BankShotSatisfied() => spawned.AllObligatorySatisfied();

        public bool RequiresBankShot() => spawned.HasAnyObligatory();

        public bool CanCreditMake()
        {
            if (!RequiresBankShot())
                return true;

            return BankShotSatisfied();
        }
        private bool TrySpawnOne(
            ObstaclePlacement placement,
            Transform throwZone,
            Transform basket)
        {
            if (placement == null || placement.obstacle == null || placement.obstacle.prefab == null)
                return false;

            Vector3Int cell = ClampCell(placement.cell);
            if (TryPlace(placement, throwZone, basket, cell))
                return true;

            Vector3Int mirror = GridPose.MirrorLateral(cell);
            if (mirror == cell)
                return false;

            return TryPlace(placement, throwZone, basket, mirror);
        }

        private bool TryPlace(
            ObstaclePlacement placement,
            Transform throwZone,
            Transform basket,
            Vector3Int cell)
        {
            Vector3 position = GridPose.WorldPosition(throwZone, basket, cell);
            ObstacleBehaviour prefab = placement.obstacle.prefab;
            if (!prefab.FitsVerticalSpan &&
                Physics.CheckSphere(position, prefab.ClearanceRadius, occlusionMask))
                return false;

            ObstacleBehaviour instance = Instantiate(
                prefab,
                position,
                Quaternion.identity,
                transform);
            instance.Configure(placement.obligatory);
            if (!instance.TryApplyVerticalFit(occlusionMask))
            {
                Destroy(instance.gameObject);
                return false;
            }

            FaceBasket(instance.transform, basket);
            spawned.Add(instance);
            return true;
        }

        private static void FaceBasket(Transform obstacle, Transform basket)
        {
            Vector3 look = basket.position - obstacle.position;
            look.y = 0f;
            if (look.sqrMagnitude > 0.0001f)
                obstacle.rotation = Quaternion.LookRotation(look.normalized, Vector3.up);
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
