using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class PuzzleThumbnailController : MonoBehaviour
    {
        private Button button;
        private Image image;

        private void Awake()
        {
            button = GetComponent<Button>();
            image = transform.Find("Image").GetComponent<Image>();
        }

        public void Initialize(PuzzleDataBase data)
        {
            if (!data.Thumbnail.TryGetValue(GameProgressData.GetPuzzleProgress(data.Id), out var sprite))
            {
                sprite = data.Thumbnail.First().Value;
            }
            image.sprite = sprite;

            button.onClick.AddListener(() => {
                TypeEventSystem.Global.Send(new Dictionary.CallForPuzzleEvent(sprite));
            });
        }
    }
}
