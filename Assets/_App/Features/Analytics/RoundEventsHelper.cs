using System.Linq;
using DigitalLove.Analytics;
using DigitalLove.Casual.Flow;
using DigitalLove.DataAccess;
using Reflex.Attributes;
using UnityEngine;

namespace DigitalLove.Game.Analytics
{
    public class RoundEventsHelper : MonoBehaviour
    {
        [SerializeField] private LevelSelector levelSelector;

        [Inject] private IAnalyticsProvider analyticsProvider;
        [Inject] private MemoryDataClient memoryDataClient;

        public void SendBasketHasBeenSpawnedEvent(float distanceToBasket)
        {
            AnalyticsEvent analyticsEvent = GetAnalyticsEvent("basket_has_been_spawned");
            analyticsEvent.AddLabel("distance_to_basket", distanceToBasket);
            SendEvent(analyticsEvent);
        }

        public void SendHasSeenThrowZoneEvent()
        {
            SendEvent(GetAnalyticsEvent("has_seen_throw_zone"));
        }

        public void SendHasGrabbedBallEvent()
        {
            SendEvent(GetAnalyticsEvent("has_grabbed_ball"));
        }

        public void SendHasScoredEvent()
        {
            SendEvent(GetAnalyticsEvent("has_scored"));
        }

        private AnalyticsEvent GetAnalyticsEvent(string key)
        {
            AnalyticsEvent analyticsEvent = new AnalyticsEvent(key);
            Play play = memoryDataClient.Get<Play>();
            analyticsEvent.AddLabel("level_id", levelSelector.GetCurrent().GetIdWithRound(play));
            return analyticsEvent;
        }

        private void SendEvent(AnalyticsEvent analyticsEvent)
        {
            if (HasToSend(analyticsEvent.key))
            {
                memoryDataClient.Get<Round>().events.Add(analyticsEvent.key);
                analyticsProvider.Send(analyticsEvent);
            }
        }

        private bool HasToSend(string key)
        {
            Round round = memoryDataClient.Get<Round>();
            return round != null && !round.events.Any(e => string.Equals(e, key));
        }
    }
}