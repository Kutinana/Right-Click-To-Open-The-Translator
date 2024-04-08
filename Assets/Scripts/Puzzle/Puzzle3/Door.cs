using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;

namespace Puzzle.Puzzle3
{
    public class Door : MonoBehaviour
    {
        public Collider2D col;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
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

            while (Input.GetMouseButton(0))
            {
                Vector3 res = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) + m_Offset;

                if (res.x >= 0) res = new Vector3(0, res.y, res.z);
                
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