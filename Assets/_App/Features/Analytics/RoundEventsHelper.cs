using System.Linq;
using DigitalLove.Analytics;
using DigitalLove.DataAccess;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game.Analytics
{
    public class RoundEventsHelper : MonoBehaviour
    {
        [Inject] private IAnalyticsProvider analyticsProvider;
        [Inject] private MemoryDataClient memoryDataClient;

        public void SendBasketBeenSpawnedEvent(string levelId)
        {
            SendEvent("basket_has_been_spawned", levelId);
        }

        public void SendHasGrabbedBallEvent(string levelId)
        {
            SendEvent("has_grabbed_ball", levelId);
        }

        public void SendHasScoredEvent(string levelId)
        {
            SendEvent("has_scored", levelId);
        }

        private void SendEvent(string key, string levelId)
        {
            if (HasToSend(key))
            {
                memoryDataClient.Get<Round>().events.Add(key);
                analyticsProvider.Send(new AnalyticsEvent(key).AddLabel("level_id", levelId));
            }
        }

        private bool HasToSend(string key)
        {
            Round round = memoryDataClient.Get<Round>();
            return round != null && !round.events.Any(e => string.Equals(e, key));
        }
    }
}