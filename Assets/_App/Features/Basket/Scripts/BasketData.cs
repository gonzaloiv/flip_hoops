using UnityEngine;

namespace DigitalLove.Game.Basket
{
    [CreateAssetMenu(fileName = "BasketData", menuName = "DigitalLove/Game/BasketData")]
    public class BasketData : ScriptableObject
    {
        public string id;
        public BasketBehaviour prefab;
    }
}
