using System;
using System.Collections.Generic;
using DigitalLove.Global;
using UnityEngine;

namespace DigitalLove.Game.Modifiers
{
    public class ModifiersSpawner : MonoBehaviour
    {
        private const float startSecsDelay = 5;
        private const float stopSecsDelay = 28;

        [SerializeField] Transform basketTransform; // ? This is the main basket transform
        [SerializeField] Transform throwZoneTransform; // ? This is the main basket transform
        [SerializeField] private DebugBool forceSpawn;

        private List<ModifierDataPercentagePair> pairs;
        private List<ModifierBehaviour> spawned = new();

        public Action<int> scored = (score) => { };

        public void DoStart(List<ModifierDataPercentagePair> pairs)
        {
            this.pairs = pairs;
            Spawn();
            // this.InvokeAfterSecs(forceSpawn.Value ? 0 : startSecsDelay, Spawn);
            // this.InvokeAfterSecs(stopSecsDelay, Unspawn);
        }

        private void Spawn()
        {
            foreach (ModifierDataPercentagePair pair in pairs)
            {
                if (pair.HasToSpawn)
                {
                    ModifierBehaviour modifier = Instantiate(pair.modifier.prefab, transform).GetComponent<ModifierBehaviour>();
                    spawned.Add(modifier);
                    modifier.Init(throwZoneTransform, basketTransform, scored);
                }
            }
        }

        private void Unspawn()
        {
            foreach (ModifierBehaviour m in spawned)
            {
                Destroy(m.gameObject);
            }
            spawned.Clear();
        }

        public void DoStop()
        {
            Unspawn();
            StopAllCoroutines();
        }
    }
}