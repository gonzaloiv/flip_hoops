using Meta.XR.MRUtilityKit;
using UnityEngine;

namespace DigitalLove.Game.Court
{
    [CreateAssetMenu(fileName = "GravityData", menuName = "DigitalLove/Game/GravityData")]
    public class GravityData : ScriptableObject
    {
        public const float Force = 6;

        public MRUKAnchor.SceneLabels sceneLabels;
        public MRUK.SurfaceType surfaceTypes;
    }
}