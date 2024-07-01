using System.Collections;
using System.Collections.Generic;
using Cameras;
using QFramework;
using UnityEngine;

namespace Puzzle.Overture.Bottle
{
    public class Bottle : MonoBehaviour
    {
        public BottleType bottleType;
        public int CurrentPosition;

        public Collider2D col;

        Vector3 m_Offset;
        Vector3 m_TargetScreenVec;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        private IEnumerator OnMouseDown()
        {
            Puzzle.HoldingBottle = this;
            col.enabled = false;

            AudioKit.PlaySound("ItemUp");

            m_TargetScreenVec = TranslatorCameraManager.Camera.WorldToScreenPoint(transform.position);
            m_Offset = transform.position - TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3
                (Input.mousePosition.x, Input.mousePosition.y, 1f));

            while (Input.GetMouseButton(0))
            {
                Vector3 res = TranslatorCameraManager.Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) + m_Offset;
                
                transform.position = res;
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnMouseOver()
        {
            if (Puzzle.HoldingBottle != null && Puzzle.HoldingBottle != this)
            {
                MoveTo(Puzzle.HoldingBottle.CurrentPosition);
            }
        }

        private void MoveTo(int position)
        {
            col.enabled = false;
            switch (bottleType)
            {
                case BottleType.Cube when position == 0:
                    StartCoroutine(MoveToCoroutine(-300));
                    break;
                case BottleType.Cube when position == 1:
                    StartCoroutine(MoveToCoroutine(0));
                    break;
                case BottleType.Cube when position == 2:
                    StartCoroutine(MoveToCoroutine(300));
                    break;
                case BottleType.Taper when position == 0:
                    StartCoroutine(MoveToCoroutine(0));
                    break;
                case BottleType.Taper when position == 1:
                    StartCoroutine(MoveToCoroutine(300));
                    break;
                case BottleType.Taper when position == 2:
                    StartCoroutine(MoveToCoroutine(600));
                    break;
                case BottleType.Ball when position == 0:
                    StartCoroutine(MoveToCoroutine(-600));
                    break;
                case BottleType.Ball when position == 1:
                    StartCoroutine(MoveToCoroutine(-300));
                    break;
                case BottleType.Ball when position == 2:
                    StartCoroutine(MoveToCoroutine(0));
                    break;
            }
            Puzzle.Swap(this, Puzzle.HoldingBottle);

            col.enabled = true;
        }

        private IEnumerator MoveToCoroutine(int _x)
        {
            while (!Mathf.Approximately(transform.localPosition.x, _x))
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, new Vector3(_x, 0, 0), 0.1f);
            }
            transform.localPosition = new Vector3(_x, 0, 0);

            yield return null;
        }

        private void OnMouseUp()
        {
            Puzzle.ReArrangePosition();

            AudioKit.PlaySound("BottleDown");

            Puzzle.HoldingBottle = null;
            col.enabled = true;
        }
    }
}