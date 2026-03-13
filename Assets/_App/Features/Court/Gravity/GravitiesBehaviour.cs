using System.Collections.Generic;
using System.Linq;
using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    public class GravityBehaviour : MonoBehaviour
    {
        [SerializeField] private GravityData[] gravities;
        [SerializeField] private EffectMesh wallsMesh;

        [Header("Debug")]
        [SerializeField] private GravityData current;

        public GravityData GetGravity(GravityFilter filter)
        {
            if (filter == GravityFilter.OnTheFloor)
            {
                return gravities.First(g => g.surfaceTypes == MRUK.SurfaceType.FACING_UP);
            }
            else
            {
                if (current == null || gravities.Length == 1)
                {
                    current = gravities[Random.Range(0, gravities.Length)];
                }
                else
                {
                    List<GravityData> selection = gravities.Where(g => g != current).ToList();
                    current = selection[Random.Range(0, selection.Count)];
                }
            }
            CreateMesh();
            return current;
        }

        private void CreateMesh()
        {
            if (wallsMesh == null)
                return;
            wallsMesh.MeshMaterial = current.material;
            wallsMesh.CreateMesh();
        }

        public void Unspawn()
        {
            if (wallsMesh == null)
                return;
            wallsMesh.DestroyMesh();
        }
    }
}