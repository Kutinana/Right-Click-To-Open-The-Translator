using System.Collections;
using System.Collections.Generic;
using DataSystem;
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
            Puzzle3.IsHoldingDoor = true;
            col.enabled = false;

            m_TargetScreenVec = Camera.main.WorldToScreenPoint(transform.position);
            m_Offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3
                (Input.mousePosition.x, Input.mousePosition.y, 1f));

            while (Input.GetMouseButton(0) && Puzzle3.Instance.IsUnlocked)
            {
                Vector3 res = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) + m_Offset;

                if (res.x >= -1) res = new Vector3(0, res.y, res.z);
                
                transform.position = new Vector3(res.x, transform.position.y, transform.position.z);
                yield return new WaitForFixedUpdate();
            }
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
            Puzzle3.IsHoldingDoor = false;
            col.enabled = true;
        }
    }
}