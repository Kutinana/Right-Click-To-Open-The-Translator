using System.Collections;
using System.Collections.Generic;
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
            image.sprite = data.Thumbnail;

            button.onClick.AddListener(() => {
                TypeEventSystem.Global.Send(new Dictionary.CallForPuzzleEvent(data));
            });
        }
    }
}
