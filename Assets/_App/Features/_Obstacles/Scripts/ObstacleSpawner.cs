using System.Collections.Generic;
using DigitalLove.Game.BankShot;
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

        public void TrySpawnAll(
            ObstaclePlacement[] placements,
            Transform throwZone,
            Transform basket)
        {
            Clear();
            if (placements == null || placements.Length == 0)
                return;

            for (int i = 0; i < placements.Length; i++)
            {
                if (!TrySpawnOne(placements[i], throwZone, basket))
                    continue;
            }
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

        public bool RequiresBankShot() => spawned.HasAnyObligatory();

        public bool CanCreditMake() => spawned.CanCreditMake();

        private bool TrySpawnOne(
            ObstaclePlacement placement,
            Transform throwZone,
            Transform basket)
        {
            if (placement == null || placement.obstacle == null || placement.obstacle.prefab == null)
                return false;

            Vector3Int cell = ThrowPathCellPose.ClampCell(placement.cell);
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
                ThrowPathCellPose.FaceBasket(position, basket),
                transform);
            instance.Configure(placement.obligatory);
            if (!instance.TryApplyVerticalFit(occlusionMask))
            {
                Destroy(instance.gameObject);
                return false;
            }

            spawned.Add(instance);
            return true;
        }
    }
}
