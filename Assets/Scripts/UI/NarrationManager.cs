using System.Collections;
using System.Collections.Generic;
using Kuchinashi;
using Localization;
using QFramework;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace UI
{
    public struct OnNarrationStartEvent {}
    public struct OnNarrationEndEvent {}
    public struct OnInitialNarrationStartEvent {}

    public class NarrationManager : MonoSingleton<NarrationManager>
    {
        private CanvasGroup mCanvasGroup;
        private TMP_Text mText;

        private static Coroutine CurrentCoroutine;

        public string ToShowNarrationId { get; set; }
        public static bool IsNarrating => CurrentCoroutine != null;

        private void Awake()
        {
            mCanvasGroup = GetComponent<CanvasGroup>();
            mText = transform.Find("Text").GetComponent<TMP_Text>();

            TypeEventSystem.Global.Register<OnInitialNarrationStartEvent>(e => {
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

            TypeEventSystem.Global.Send(new OnNarrationStartEvent());
        }

        public static void StopNarration()
        {
            Instance.StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Instance.mCanvasGroup, 0f));

            if (CurrentCoroutine != null) Instance.StopCoroutine(CurrentCoroutine);
            CurrentCoroutine = null;

            TypeEventSystem.Global.Send(new OnNarrationEndEvent());
        }

        private IEnumerator NarrationCoroutine(List<string> content, float delay = 0f)
        {
            mText.SetText("");
            yield return new WaitForSeconds(delay);

            yield return CanvasGroupHelper.FadeCanvasGroup(Instance.mCanvasGroup, 1f);
            
            foreach (var text in content)
            {
                mText.SetText("");

                var len = text.Length;
                var speed = 1 / 24f;
                
                for (var i = 0; i < len; i++)
                {
                    mText.text += text[i];
                    if (text[i] is not ' ' or '\r' or '\n') AudioKit.PlaySound("InteractClick");
                    yield return new WaitForSeconds(speed);
                }
                mText.SetText(text);
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

            EditorGUILayout.BeginHorizontal();
            manager.ToShowNarrationId = EditorGUILayout.TextField(manager.ToShowNarrationId);
            if (GUILayout.Button("Start"))
            {
                NarrationManager.StartNarration(manager.ToShowNarrationId);
            }
            EditorGUILayout.EndHorizontal();
        }
    }

    #endif
}