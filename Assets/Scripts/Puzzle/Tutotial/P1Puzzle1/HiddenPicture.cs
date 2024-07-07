using Cameras;
using Kuchinashi;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;


namespace Puzzle.Tutorial.P1
{
    public class HiddenPicture: MonoBehaviour
    {
        Button Left;
        Button Middle;
        Button Right;

        Transform CoverPicture;
        Collider2D CoverCollider;
        CanvasGroup Scopes;

        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] public Vector3 targetPosition = Vector3.zero;
        [Range(0, 1)] public float Progress = 0f;

        [Range(0, 2)] private int stage = 0;

        private Coroutine CurrentCoroutine = null;
        private List<Transform> characters;

        private void Awake()
        {
            CoverPicture = transform.Find("CoverPicture");
            CoverCollider = transform.GetComponent<Collider2D>();
            Scopes = transform.Find("Hidden/Scopes").GetComponent<CanvasGroup>();
            
            Left = transform.Find("Hidden/Buttons/Left").GetComponent<Button>();
            Left.onClick.AddListener(() => Puzzle.PatternUpdate(0, Left.transform));
    
            Middle = transform.Find("Hidden/Buttons/Middle").GetComponent<Button>();
            Middle.onClick.AddListener(() => Puzzle.PatternUpdate(1, Middle.transform));
    
            Right = transform.Find("Hidden/Buttons/Right").GetComponent<Button>();
            Right.onClick.AddListener(() => Puzzle.PatternUpdate(2, Right.transform));

            characters = new List<Transform>()
            {
                transform.Find("Hidden/Characters/Left"),
                transform.Find("Hidden/Characters/Middle"),
                transform.Find("Hidden/Characters/Right")
            };

            Puzzle.S1Correct += delegate
            {
                Stage1Correct();
            };
            Puzzle.Show += ShowCharacter;
            Puzzle.Hide += HideCharacter;

            if (stage == 0)
            {
                Left.interactable = false;
                Middle.interactable = false;
                Right.interactable = false;
            }
        }

        private void NextStage()
        {
            CanvasGroup Buttons;
            switch (stage)
            {
                case 0:
                    Buttons = transform.Find("Hidden/Buttons").GetComponent<CanvasGroup>();
                    Buttons.alpha = 1;
                    Buttons.blocksRaycasts = true;
                    Left.interactable = true;
                    Middle.interactable = true;
                    Right.interactable = true;
                    stage++;
                    break;
                case 1:
                    Left.interactable = false;
                    Middle.interactable = false;
                    Right.interactable = false;
                    Buttons = transform.Find("Hidden/Buttons").GetComponent<CanvasGroup>();
                    Buttons.alpha = 0;
                    Buttons.blocksRaycasts = false;
                    stage++;
                    break;
                case 2:
                    break;
                default:
                    break;
            }
        }

        private void Stage1Correct()
        {
            _initialPosition = Scopes.transform.localPosition;
            _targetPosition = new Vector3(_initialPosition.x, _initialPosition.y + 300, _initialPosition.z);
            CurrentCoroutine = StartCoroutine(PopScopes());
            NextStage();
        }

        Vector3 m_Offset;
        Vector3 m_TargetScreenVec;

        private IEnumerator OnMouseDown()
        {
            if (CurrentCoroutine != null) yield break;
            CoverCollider.enabled = false;

            m_TargetScreenVec = TranslatorCameraManager.Camera.WorldToScreenPoint(transform.position);
            m_Offset = transform.position - TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3
                (Input.mousePosition.x, Input.mousePosition.y, 1f));

            while (Input.GetMouseButton(0) && stage == 0)
            {
                Vector3 res = TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) + m_Offset;

                if (res.y > 1)
                {
                    CurrentCoroutine = StartCoroutine(MoveToCoroutine());
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnMouseUp()
        {
            if (stage == 0) CoverCollider.enabled = true;
        }

        float c_parameter = 0f;
        Color _initialColor;
        Color _targetColor;

        private void ShowCharacter(object sender, int id)
        {
            c_parameter = 0f;
            _initialColor = characters[id].Find("Image").GetComponent<Image>().color;
            _targetColor = new Color(_initialColor.r, _initialColor.g, _initialColor.b, 1f);
            CurrentCoroutine = StartCoroutine(ChangeCharacter(id));
        }
        private void HideCharacter(object sender, int id)
        {
            c_parameter = 0f;
            _initialColor = characters[id].Find("Image").GetComponent<Image>().color;
            _targetColor = new Color(_initialColor.r, _initialColor.g, _initialColor.b, 0f);
            CurrentCoroutine = StartCoroutine(ChangeCharacter(id));
        }

        private IEnumerator ChangeCharacter(int id)
        {
            while (c_parameter < 0.99f)
            {
                characters[id].Find("Image").GetComponent<Image>().color = Color.Lerp(_initialColor, _targetColor, c_parameter);
                c_parameter += Time.deltaTime * 1.5f;

                yield return new WaitForFixedUpdate();
            }
            characters[id].Find("Image").GetComponent<Image>().color = _targetColor;
            characters[id].GetComponent<ButtonExtension>().interactable = true;

            CurrentCoroutine = null;
        }

        private IEnumerator MoveToCoroutine()
        {
            Color current_color = CoverPicture.GetComponent<Image>().color;
            Debug.Log("start Coroutine");

            while (Progress < 0.99f)
            {
                CoverPicture.localPosition = targetPosition * animationCurve.Evaluate(Progress);
                CoverPicture.GetComponent<Image>().color = new Color(current_color.r, current_color.g, current_color.b, 1 - Progress);
                Progress += Time.deltaTime * 1f;

                yield return new WaitForFixedUpdate();
            }
            CoverPicture.localPosition = targetPosition;
            CoverPicture.GetComponent<Image>().color = new Color(current_color.r, current_color.g, current_color.b, 0);

            CurrentCoroutine = null;
            NextStage();
        }

        float parameter = 0f;
        Vector3 _initialPosition;
        Vector3 _targetPosition;

        private IEnumerator PopScopes()
        {
            while (parameter < 0.99f)
            {
                Scopes.alpha = Mathf.Lerp(0, 1, parameter);
                Scopes.transform.localPosition = Vector3.Lerp(_initialPosition, _targetPosition, parameter);
                parameter += Time.deltaTime * 0.6f;

                yield return new WaitForFixedUpdate();
            }
            Scopes.alpha = 1;

            CurrentCoroutine = null;
        }
    }
}
