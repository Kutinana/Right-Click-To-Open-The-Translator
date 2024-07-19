using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UnityEngine;
using UI;
using UnityEngine.UI;
using Kuchinashi;
using TMPro;
using System.Linq;
using Localization;

namespace Translator
{
    public struct OnItemDataUpdateEvent { public ObtainableObjectData Data; }
    public struct OnItemUseEvent { public ObtainableObjectData Data; }
    public class BackpackController : MonoSingleton<BackpackController>
    {
        public GameObject ObtainableItemPrefab;
        public Coroutine CurrentCoroutine = null;

        internal CanvasGroup itemListCanvasGroup;
        internal CanvasGroup detailCanvasGroup;

        private Image currentItemImage;
        private TMP_Text currentItemName;
        private TMP_Text currentItemDes;
        private TMP_Text currentItemSubDes;

        private Button useButton;

        public ObtainableObjectData CurrentItemData;

        private void Awake()
        {
            itemListCanvasGroup = transform.Find("ItemList").GetComponent<CanvasGroup>();
            detailCanvasGroup = transform.Find("ItemList/Detail").GetComponent<CanvasGroup>();

            currentItemImage = detailCanvasGroup.transform.Find("Item").GetComponent<Image>();
            currentItemName = detailCanvasGroup.transform.Find("Name").GetComponent<TMP_Text>();
            currentItemDes = detailCanvasGroup.transform.Find("Description").GetComponent<TMP_Text>();
            currentItemSubDes = detailCanvasGroup.transform.Find("SubDescription").GetComponent<TMP_Text>();

            useButton = detailCanvasGroup.transform.Find("Use").GetComponent<Button>();
            useButton.onClick.AddListener(() =>
            {
                if (CurrentItemData == null) return;
                TypeEventSystem.Global.Send(new OnItemUseEvent { Data = CurrentItemData });
                
                TranslatorSM.StateMachine.ChangeState(States.Off);
            });

            TypeEventSystem.Global.Register<OnItemDataUpdateEvent>(e => {
                if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);

                CurrentItemData = e.Data;
                CurrentCoroutine = StartCoroutine(ShowItemDetailCoroutine());
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public void GenerateItemList()
        {
            var parent = transform.Find("ItemList/Scroll View/Viewport/Content");
            for (var i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }

            if (GameProgressData.GetInventory().IsNullOrEmpty()) return;
            foreach (var c in GameProgressData.GetInventory())
            {
                if (string.IsNullOrEmpty(c.Key) || c.Value == 0) continue;

                var go = Instantiate(ObtainableItemPrefab, parent);
                go.GetComponent<ObtainableObjectController>().Initialize(GameDesignData.GetObtainableObjectDataById(c.Key));
            }
        }

        public IEnumerator ShowItemDetailCoroutine()
        {
            yield return CanvasGroupHelper.FadeCanvasGroup(detailCanvasGroup, 0f, 0.2f);

            currentItemImage.sprite = CurrentItemData.Sprite;
            currentItemName.SetText(LocalizationHelper.Get(CurrentItemData.Name));
            currentItemDes.SetText(LocalizationHelper.Get(CurrentItemData.Description));
            currentItemSubDes.SetText(LocalizationHelper.Get(CurrentItemData.SubDescription));

            yield return CanvasGroupHelper.FadeCanvasGroup(detailCanvasGroup, 1f, 0.2f);
        }
    }
}