using System.Collections;
using System.Collections.Generic;
using Cameras;
using DataSystem;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Puzzle3
{
    public class Door : MonoBehaviour
    {
        public Collider2D col;

        private Button LeftUp;
        private Button LeftDown;
        private Button MiddleUp;
        private Button MiddleDown;
        private Button RightUp;
        private Button RightDown;

        [SerializeField] private AnimationCurve animationCurve = AnimationCurve.Linear(0, 0, 1, 1);
        [SerializeField] private Vector3 targetPosition = Vector3.zero;
        [Range(0, 1)] public float Progress = 0f;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            col = GetComponent<Collider2D>();

            LeftUp = transform.Find("LeftButtonUp").GetComponent<Button>();
            LeftUp.onClick.AddListener(() => Puzzle3.UpdateNumber(0, 1));
            LeftDown = transform.Find("LeftButtonDown").GetComponent<Button>();
            LeftDown.onClick.AddListener(() => Puzzle3.UpdateNumber(0, -1));

            MiddleUp = transform.Find("MiddleButtonUp").GetComponent<Button>();
            MiddleUp.onClick.AddListener(() => Puzzle3.UpdateNumber(1, 1));
            MiddleDown = transform.Find("MiddleButtonDown").GetComponent<Button>();
            MiddleDown.onClick.AddListener(() => Puzzle3.UpdateNumber(1, -1));

            RightUp = transform.Find("RightButtonUp").GetComponent<Button>();
            RightUp.onClick.AddListener(() => Puzzle3.UpdateNumber(2, 1));
            RightDown = transform.Find("RightButtonDown").GetComponent<Button>();
            RightDown.onClick.AddListener(() => Puzzle3.UpdateNumber(2, -1));
        }
        
        Vector3 m_Offset;
        Vector3 m_TargetScreenVec;

        private IEnumerator OnMouseDown()
        {
            if (CurrentCoroutine != null) yield break;

            Puzzle3.IsHoldingDoor = true;
            col.enabled = false;

            m_TargetScreenVec = TranslatorCameraManager.Camera.WorldToScreenPoint(transform.position);
            m_Offset = transform.position - TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3
                (Input.mousePosition.x, Input.mousePosition.y, 1f));

            while (Input.GetMouseButton(0) && Puzzle3.Instance.IsUnlocked)
            {
                Vector3 res = TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) + m_Offset;

                if (res.x < -1)
                {
                    CurrentCoroutine = StartCoroutine(MoveToCoroutine());
                    yield break;
                }
                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator MoveToCoroutine()
        {
            AudioKit.PlaySound("DoorOpen1");
            while (Progress < 0.99f)
            {
                transform.localPosition = targetPosition * animationCurve.Evaluate(Progress);
                Progress += Time.deltaTime * 0.3f;

                yield return new WaitForFixedUpdate();
            }
            transform.localPosition = targetPosition;

            CurrentCoroutine = null;
            PuzzleManager.Solved();
        }

        private void OnMouseUp()
        {
            Puzzle3.IsHoldingDoor = false;
            col.enabled = true;
        }
    }
}