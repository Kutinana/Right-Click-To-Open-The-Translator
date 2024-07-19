using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Localization;
using QFramework;
using TMPro;
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

    public enum Narrator
    {
        Protagonist,
        Shiro
    }

    public enum Side
    {
        Left,
        Right
    }

    public class NarrationManager : MonoSingleton<NarrationManager>
    {
        public SerializableDictionary<Narrator, Sprite> Tachies;

        private CanvasGroup mCanvasGroup;
        private string narrationPlaying = null;
        private static Coroutine CurrentCoroutine;

        private CanvasGroup leftCanvasGroup;
        private Image leftImage;
        private TMP_Text leftText;
        private CanvasGroup rightCanvasGroup;
        private Image rightImage;
        private TMP_Text rightText;

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

            TypeEventSystem.Global.Register<OnInitialNarrationStartEvent>(e =>
            {
                if (PlayerPrefs.HasKey("FirstTime") && PlayerPrefs.GetInt("FirstTime") == 1)
                {
                    StartNarration("InitialNarration", 2f);
                }
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (IsNarrating) StopNarration();
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
                if (sentence.side == Side.Left)
                {
                    yield return CanvasGroupHelper.FadeCanvasGroup(Instance.rightCanvasGroup, 0f, 0.2f);

                    leftText.SetText("");
                    leftImage.sprite = Tachies[sentence.narrator];

                    yield return CanvasGroupHelper.FadeCanvasGroup(Instance.leftCanvasGroup, 1f, 0.2f);
                    
                    var len = sentence.content.Length;
                    var speed = 1 / 24f;

                    for (var i = 0; i < len; i++)
                    {
                        leftText.text += sentence.content[i];
                        if (sentence.content[i] is not ' ' or '\r' or '\n') AudioKit.PlaySound("InteractClick");
                        yield return new WaitForSeconds(speed);
                    }
                    leftText.SetText(sentence.content);
                }
                else if (sentence.side == Side.Right)
                {
                    yield return CanvasGroupHelper.FadeCanvasGroup(Instance.leftCanvasGroup, 0f, 0.2f);

                    rightText.SetText("");
                    rightImage.sprite = Tachies[sentence.narrator];

                    yield return CanvasGroupHelper.FadeCanvasGroup(Instance.rightCanvasGroup, 1f, 0.2f);
                    
                    var len = sentence.content.Length;
                    var speed = 1 / 24f;

                    for (var i = 0; i < len; i++)
                    {
                        rightText.text += sentence.content[i];
                        if (sentence.content[i] is not ' ' or '\r' or '\n') AudioKit.PlaySound("InteractClick");
                        yield return new WaitForSeconds(speed);
                    }
                    rightText.SetText(sentence.content);
                }
                
                yield return new WaitForSeconds(0.5f);

                yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
            }

            yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyUp(KeyCode.Return));
            StopNarration();
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