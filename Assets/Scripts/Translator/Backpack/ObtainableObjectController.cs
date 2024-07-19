using System.Collections;
using System.Collections.Generic;
using Translator;
using QFramework;
using UnityEngine;
using DataSystem;
using UnityEngine.UI;

namespace UI
{
    public class ObtainableObjectController : MonoBehaviour
    {
        public ObtainableObjectData data;

        protected Image _image;
        protected Button _button;

        private void Awake()
        {
            _image = transform.Find("Image").GetComponent<Image>();
            _button = GetComponent<Button>();

            _button.onClick.AddListener(() => {
                TypeEventSystem.Global.Send(new OnItemDataUpdateEvent() { Data = data });
            });
        }

        public virtual void Initialize(ObtainableObjectData _data = null)
        {
            if (_data == null) return;
            
            data = _data;
            _image.sprite = data.Sprite;
        }
    }
}