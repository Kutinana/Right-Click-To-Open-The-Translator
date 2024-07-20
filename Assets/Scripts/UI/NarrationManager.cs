using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Localization;
using QFramework;
using TMPro;
using Translator;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Narration
{
    public struct OnNarrationStartEvent
    {
        public OnNarrationStartEvent(string id) : this()
        {
            Id = id;
        }

        public string Id { get; }
    }
    public struct OnNarrationEndEvent
    {
        public OnNarrationEndEvent(string id) : this()
        {
            Id = id;
        }

        public string Id { get; }

    }
    public struct OnInitialNarrationStartEvent { }

    public enum Side
    {
        Left,
        Right,
        FullScreen
    }

    public class NarrationManager : MonoSingleton<NarrationManager>
    {
        public SerializableDictionary<string, Sprite> Tachies;

        private CanvasGroup mCanvasGroup;
        private string narrationPlaying = null;
        private static Coroutine CurrentCoroutine;

        private static Coroutine CurrentTypeTextCoroutine;

        private CanvasGroup leftCanvasGroup;
        private Image leftImage;
        private TMP_Text leftText;
        private CanvasGroup rightCanvasGroup;
        private Image rightImage;
        private TMP_Text rightText;
        private CanvasGroup fullScreenCanvasGroup;
        private Image fullScreenImage;
        private TMP_Text fullScreenText;

        public string ToShowNarrationId { get; set; }
        public static bool IsNarrating => CurrentCoroutine != null;

        private void Awake()
        {
            mCanvasGroup = GetComponent<CanvasGroup>();

            leftCanvasGroup = transform.Find("Left").GetComponent<CanvasGroup>();
            leftImage = transform.Find("Left/Tachie").GetComponent<Image>();
            leftText = transform.Find("Left/Text").GetComponent<TMP_Text>();

            rightCanvasGroup = transform.Find("Right").GetComponent<CanvasGroup>();
            rightImage = transform.Find("Right/Tachie").GetComponent<Image>();
            rightText = transform.Find("Right/Text").GetComponent<TMP_Text>();

            fullScreenCanvasGroup = transform.Find("FullScreen").GetComponent<CanvasGroup>();
            fullScreenImage = transform.Find("FullScreen/Tachie").GetComponent<Image>();
            fullScreenText = transform.Find("FullScreen/Text").GetComponent<TMP_Text>();

            EventRegister();
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (IsNarrating) StopNarration();
            }
            if (Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return))
            {
                if (CurrentTypeTextCoroutine != null) StopCoroutine(CurrentTypeTextCoroutine);
                CurrentTypeTextCoroutine = null;
            }
        }

        public static void StartNarration(string id, float delay = 0f)
        {
            if (string.IsNullOrEmpty(id)) return;

            var content = LocalizationManager.GetNarration(id);

            if (CurrentCoroutine != null) Instance.StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = Instance.StartCoroutine(Instance.NarrationCoroutine(content, delay));
            Instance.narrationPlaying = id;
            TypeEventSystem.Global.Send(new OnNarrationStartEvent(id));
        }

        public static void StopNarration()
        {
            Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.mCanvasGroup, 0f));

            if (CurrentCoroutine != null) Instance.StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;
            TypeEventSystem.Global.Send(new OnNarrationEndEvent(Instance.narrationPlaying));
            Instance.narrationPlaying = null;
        }

        private IEnumerator NarrationCoroutine(List<NarrationSentence> content, float delay = 0f)
        {
            yield return new WaitForSeconds(delay);

            leftCanvasGroup.alpha = 0;
            rightCanvasGroup.alpha = 0;
            yield return CanvasGroupHelper.FadeCanvasGroup(Instance.mCanvasGroup, 1f);

            foreach (var sentence in content)
            {
                if (sentence.type == Side.Left)
                {
                    yield return CanvasGroupHelper.FadeCanvasGroup(new CanvasGroup[] {Instance.rightCanvasGroup, Instance.fullScreenCanvasGroup}, 0f, 0.2f);

                    leftText.SetText("");
                    if (!string.IsNullOrEmpty(sentence.narrator) && Tachies.TryGetValue(sentence.narrator, out var value))
                    {
                        leftImage.sprite = value;
                        leftImage.color = Color.white;
                    }
                    else leftImage.color = new Color(1f, 1f, 1f, 0f);

                    yield return CanvasGroupHelper.FadeCanvasGroup(Instance.leftCanvasGroup, 1f, 0.2f);
                    
                    CurrentTypeTextCoroutine = StartCoroutine(TypeTextCoroutine(leftText, sentence.content));
                    yield return new WaitUntil(() => CurrentTypeTextCoroutine == null);
                    leftText.SetText(sentence.content);
                }
                else if (sentence.type == Side.Right)
                {
                    yield return CanvasGroupHelper.FadeCanvasGroup(new CanvasGroup[] {Instance.leftCanvasGroup, Instance.fullScreenCanvasGroup}, 0f, 0.2f);

                    rightText.SetText("");
                    if (!string.IsNullOrEmpty(sentence.narrator) && Tachies.TryGetValue(sentence.narrator, out var value))
                    {
                        rightImage.sprite = value;
                        rightImage.color = Color.white;
                    }
                    else rightImage.color = new Color(1f, 1f, 1f, 0f);

                    yield return CanvasGroupHelper.FadeCanvasGroup(Instance.rightCanvasGroup, 1f, 0.2f);
                    
                    CurrentTypeTextCoroutine = StartCoroutine(TypeTextCoroutine(rightText, sentence.content));
                    yield return new WaitUntil(() => CurrentTypeTextCoroutine == null);
                    rightText.SetText(sentence.content);
                }
                else if (sentence.type == Side.FullScreen)
                {
                    yield return CanvasGroupHelper.FadeCanvasGroup(new CanvasGroup[] {Instance.leftCanvasGroup, Instance.rightCanvasGroup}, 0f, 0.2f);

                    fullScreenText.SetText("");
                    fullScreenImage.color = new Color(1f, 1f, 1f, 0f);

                    yield return CanvasGroupHelper.FadeCanvasGroup(Instance.fullScreenCanvasGroup, 1f, 0.2f);
                    
                    CurrentTypeTextCoroutine = StartCoroutine(TypeTextCoroutine(fullScreenText, sentence.content));
                    yield return new WaitUntil(() => CurrentTypeTextCoroutine == null);
                    fullScreenText.SetText(sentence.content);
                }

                yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
            }

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
            StopNarration();
        }

        private IEnumerator TypeTextCoroutine(TMP_Text textfield, string text)
        {
            var len = text.Length;
            var speed = 1 / 24f;

            for (var i = 0; i < len; i++)
            {
                textfield.text += text[i];
                if (text[i] is not ' ' or '\r' or '\n') AudioKit.PlaySound("InteractClick");
                yield return new WaitForSeconds(speed);
            }
            textfield.SetText(text);

            CurrentTypeTextCoroutine = null;
        }

        private void EventRegister()
        {
            TypeEventSystem.Global.Register<OnInitialNarrationStartEvent>(e =>
            {
                if (PlayerPrefs.HasKey("FirstTime") && PlayerPrefs.GetInt("FirstTime") == 1)
                {
                    StartNarration("InitialNarration", 2f);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);

            TypeEventSystem.Global.Register<OnItemUseEvent>(e =>
            {
                switch (e.Data.Id)
                {
                    case "disk1":
                        StartNarration("Disk1");
                        break;
                    case "disk2":
                        StartNarration("Disk2");
                        break;
                    case "disk3":
                        StartNarration("Disk3");
                        break;
                    case "disk4":
                        StartNarration("Disk4");
                        break;
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(NarrationManager))]
    [CanEditMultipleObjects]
    public class NarrationManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            NarrationManager manager = (NarrationManager)target;

            EditorGUILayout.Separator();

            manager.ToShowNarrationId = EditorGUILayout.TextField(manager.ToShowNarrationId);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Start"))
            {
                NarrationManager.StartNarration(manager.ToShowNarrationId);
            }
            if (GUILayout.Button("Stop"))
            {
                NarrationManager.StopNarration();
            }
            EditorGUILayout.EndHorizontal();
        }
    }

#endif
}