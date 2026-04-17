using System;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    [CreateAssetMenu(fileName = "ModifierData", menuName = "DigitalLove/Game/ModifierData")]
    public class ModifierData : ScriptableObject
    {
        public string id;
        public ModifierBehaviour prefab;
    }

    [Serializable]
    public class ModifierDataPercentagePair
    {
        public ModifierData modifier;
        public int percentage; // ? 0 to 100

        public bool HasToSpawn => UnityEngine.Random.Range(0, 100) < percentage;
    }
}