using Meta.XR.MRUtilityKit;
using UnityEngine;
using static Meta.XR.MRUtilityKit.FindSpawnPositions;

namespace DigitalLove.Game.Court
{
    [CreateAssetMenu(fileName = "GravityData", menuName = "DigitalLove/Game/GravityData")]
    public class GravityData : ScriptableObject
    {
        public Vector3 direction;
        public float force;
        public Material material;
        public MRUKAnchor.SceneLabels sceneLabels;
        public MRUK.SurfaceType surfaceTypes;
        public GameObject basket;
    }
}
