using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UnityEngine;
using UI;
using Translator;
using UnityEngine.UI;
using Kuchinashi;
using TMPro;
using System.Linq;

namespace Dictionary
{
    public class DictionarySM : MonoBehaviour , ISingleton
    {
        public static DictionarySM Instance => SingletonProperty<DictionarySM>.Instance;

        public GameObject CharacterPrefab;
        public GameObject PuzzlePrefab;
        public Coroutine CurrentCoroutine = null;

        internal CanvasGroup characterListCanvasGroup;
        internal CanvasGroup detailCanvasGroup;
        internal CanvasGroup puzzleCanvasGroup;

        private Image currentCharacterImage;
        private TMP_Text currentCharacterMeaning;

        private Button backToCharacterListBtn;

        public CharacterData CurrentCharacterData;

        public void OnSingletonInit() {}

        private void Awake()
        {
            characterListCanvasGroup = transform.Find("CharacterList").GetComponent<CanvasGroup>();
            detailCanvasGroup = transform.Find("CharacterList/Image/Detail").GetComponent<CanvasGroup>();
            puzzleCanvasGroup = transform.Find("Puzzle").GetComponent<CanvasGroup>();

            currentCharacterImage = detailCanvasGroup.transform.Find("Character").GetComponent<Image>();
            currentCharacterMeaning = detailCanvasGroup.transform.Find("Meaning").GetComponent<TMP_Text>();

            backToCharacterListBtn = puzzleCanvasGroup.transform.Find("Back").GetComponent<Button>();
            backToCharacterListBtn.onClick.AddListener(() =>
            {
                if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);

                CurrentCoroutine = StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(puzzleCanvasGroup, 0f, 0.1f));
            });

            TypeEventSystem.Global.Register<OnCharacterDataUpdateEvent>(e => {
                if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);

                CurrentCharacterData = e.Data;
                CurrentCoroutine = StartCoroutine(ShowCharacterDetailCoroutine());
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<CallForPuzzleEvent>(e => {
                if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);

                puzzleCanvasGroup.transform.Find("Image").GetComponent<Image>().sprite = e.Sprite;
                CurrentCoroutine = StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(puzzleCanvasGroup, 1f, 0.1f));
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnCharacterRecordedEvent>(e => {
                if (e.id == CurrentCharacterData.Id) currentCharacterMeaning.SetText(UserDictionary.Read(e.id));
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnCharacterRefreshEvent>(e => {
                currentCharacterMeaning.SetText(UserDictionary.Read(CurrentCharacterData.Id));
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public void GenerateCharacterList()
        {
            var parent = transform.Find("CharacterList/Image/Scroll View/Viewport/Content");
            for (var i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }

            if (UserDictionary.IsEmpty()) return;
            foreach (var c in UserDictionary.GetDictionary())
            {
                if (string.IsNullOrEmpty(c.Key)) continue;

                var go = Instantiate(CharacterPrefab, parent);
                go.GetComponent<DictionaryCharacterController>().Initialize(GameDesignData.GetCharacterDataById(c.Key), isInteractable: TranslatorSM.StateMachine.CurrentStateId != Translator.States.Off, isBlack: true);
            }
        }

        public IEnumerator ShowCharacterDetailCoroutine()
        {
            if (!UserDictionary.TryGetCharacterProgressData(CurrentCharacterData.Id, out var progress)) yield break;
            yield return CanvasGroupHelper.FadeCanvasGroup(detailCanvasGroup, 0f, 0.2f);

            currentCharacterImage.sprite = CurrentCharacterData.Sprite;
            currentCharacterMeaning.SetText(progress.Meaning);

            var btn = currentCharacterImage.GetComponent<Button>();
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => CharacterRecordPanelManager.Instance.Init(CurrentCharacterData));

            var parent = detailCanvasGroup.transform.Find("PuzzleList/Scroll View/Viewport/Content");
            for (var i = 0; i < parent.childCount; i++)
            {
                Destroy(parent.GetChild(i).gameObject);
            }

            foreach (var p in progress.RelatedPuzzles)
            {
                if (GameProgressData.GetPuzzleProgress(p) == PuzzleProgress.NotFound) continue;

                var go = Instantiate(PuzzlePrefab, parent);
                go.GetComponent<PuzzleThumbnailController>().Initialize(GameDesignData.GetPuzzleDataById(p));
            }

            yield return CanvasGroupHelper.FadeCanvasGroup(detailCanvasGroup, 1f, 0.2f);
        }
    }

    public struct OnCharacterDataUpdateEvent {
        public CharacterData Data;
        public OnCharacterDataUpdateEvent(CharacterData _data) { Data = _data; }
    }

    public struct CallForPuzzleEvent {
        public Sprite Sprite;
        public CallForPuzzleEvent(Sprite _sprite) { Sprite = _sprite; }
    }
}