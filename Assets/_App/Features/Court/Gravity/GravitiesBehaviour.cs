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

        public GravityData SpawnRandomGravity()
        {
            if (current == null)
            {
                current = gravities[Random.Range(0, gravities.Length)];
            }
            else
            {
                List<GravityData> selection = gravities.Where(g => g != current).ToList();
                current = selection[Random.Range(0, selection.Count)];
            }
            wallsMesh.MeshMaterial = current.material;
            wallsMesh.CreateMesh();
            return current;
        }

        public void Unspawn()
        {
            wallsMesh.DestroyMesh();
        }
    }
}