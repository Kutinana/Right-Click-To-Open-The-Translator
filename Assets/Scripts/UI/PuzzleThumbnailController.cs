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
        private TMP_Text nameText;

        private void Awake()
        {
            button = GetComponent<Button>();
            image = transform.Find("Image").GetComponent<Image>();
            nameText = transform.Find("Name").GetComponent<TMP_Text>();
        }

        public void Initialize(PuzzleDataBase data)
        {
            image.sprite = data.Thumbnail;
            nameText.SetText(data.Name);

            button.onClick.AddListener(() => {
                TypeEventSystem.Global.Send(new Dictionary.CallForPuzzleEvent(data.Type, data.Id));
            });
        }
    }
}
