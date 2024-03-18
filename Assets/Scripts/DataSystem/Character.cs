using System.Collections;
using System.Collections.Generic;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace DataSystem
{
    public class Character : MonoBehaviour
    {
        public CharacterData data;

        private Image _image;
        private TMP_Text _text;
        private Button _button;

        private void Awake()
        {
            _image = transform.Find("Image").GetComponent<Image>();
            _text = transform.Find("Text").GetComponent<TMP_Text>();
            _button = GetComponent<Button>();

            Initialize();
        }

        private void Initialize()
        {
            _image.sprite = data.Sprite;
            _text.SetText(UserDictionary.Read(data.Id));

            _button.onClick.AddListener(() => {
                CharacterRecordPanelManager.Instance.Init(this);
            });
        }

        public void Refresh()
        {
            _image.sprite = data.Sprite;
            _text.SetText(UserDictionary.Read(data.Id));
        }
    }
}