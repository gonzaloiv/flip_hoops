using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace DigitalLove.Game.UI
{
    public class LevelsPanel : MonoBehaviour
    {
        [FormerlySerializedAs("rowPrefab")]
        [SerializeField] private LevelItem itemTemplate;
        [SerializeField] private Transform content;
        [SerializeField] private LevelItem randomItem;

        public event Action<string> levelPressed;
        public event Action randomPressed;

        private readonly List<LevelItem> items = new();
        private readonly List<LevelItemData> entries = new();
        private bool interactionEnabled;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void SetInteractionEnabled(bool enabled)
        {
            interactionEnabled = enabled;
            Rebind();
        }

        public void Refresh(List<LevelItemData> nextEntries)
        {
            entries.Clear();
            if (nextEntries != null)
            {
                for (int i = 0; i < nextEntries.Count; i++)
                    entries.Add(nextEntries[i]);
            }
            Rebind();
        }

        private void Rebind()
        {
            EnsureItemCount(entries.Count);
            for (int i = 0; i < entries.Count && i < items.Count; i++)
                items[i].Bind(entries[i], OnItemPressed, interactionEnabled);
            BindRandomItem();
        }

        private void EnsureItemCount(int count)
        {
            if (itemTemplate == null || content == null)
                return;

            if (items.Count == 0)
            {
                if (count == 0)
                {
                    itemTemplate.gameObject.SetActive(false);
                    return;
                }

                items.Add(itemTemplate);
            }

            while (items.Count < count)
                items.Add(Instantiate(itemTemplate, content));

            for (int i = 0; i < items.Count; i++)
                items[i].gameObject.SetActive(i < count);

            if (randomItem != null)
                randomItem.transform.SetAsLastSibling();
        }

        private void BindRandomItem()
        {
            if (randomItem == null)
                return;
            randomItem.gameObject.SetActive(true);
            randomItem.Bind(RandomEntry(), OnRandomPressed, interactionEnabled);
        }

        private static LevelItemData RandomEntry()
        {
            return new LevelItemData
            {
                levelId = string.Empty,
                identityLabel = "RND",
                scoreText = string.Empty,
                stars = 0,
                passed = false,
                selected = false,
                frontier = false,
                locked = false
            };
        }

        private void OnItemPressed(string levelId) => levelPressed?.Invoke(levelId);

        private void OnRandomPressed(string _) => randomPressed?.Invoke();
    }
}
