using System.Collections;
using System.Collections.Generic;
using UnityEditor.Tilemaps;
using UnityEngine;

namespace Puzzle.SamplePuzzle
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
            SamplePuzzle.HoldingBottle = this;
            col.enabled = false;

            m_TargetScreenVec = Camera.main.WorldToScreenPoint(transform.position);
            m_Offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3
                (Input.mousePosition.x, Input.mousePosition.y, 1f));

            while (Input.GetMouseButton(0))
            {
                Vector3 res = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) + m_Offset;
                
                transform.position = res;
                yield return new WaitForFixedUpdate();
            }
        }

        private void OnMouseOver()
        {
            if (SamplePuzzle.HoldingBottle != null && SamplePuzzle.HoldingBottle != this)
            {
                MoveTo(SamplePuzzle.HoldingBottle.CurrentPosition);
            }
        }

        private void MoveTo(int position)
        {
            switch (bottleType)
            {
                case BottleType.Cube when position == 0:
                    StartCoroutine(MoveToCoroutine(0));
                    break;
                case BottleType.Cube when position == 1:
                    StartCoroutine(MoveToCoroutine(300));
                    break;
                case BottleType.Cube when position == 2:
                    StartCoroutine(MoveToCoroutine(600));
                    break;
                case BottleType.Taper when position == 0:
                    StartCoroutine(MoveToCoroutine(-300));
                    break;
                case BottleType.Taper when position == 1:
                    StartCoroutine(MoveToCoroutine(0));
                    break;
                case BottleType.Taper when position == 2:
                    StartCoroutine(MoveToCoroutine(300));
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
            SamplePuzzle.Swap(this, SamplePuzzle.HoldingBottle);
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
            SamplePuzzle.ReArrangePosition();

            SamplePuzzle.HoldingBottle = null;
            col.enabled = true;
        }
    }
}