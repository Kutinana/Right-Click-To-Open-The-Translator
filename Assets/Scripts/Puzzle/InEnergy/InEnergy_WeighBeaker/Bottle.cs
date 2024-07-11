using System.Collections;
using System.Collections.Generic;
using Cameras;
using DataSystem;
using QFramework;
using UnityEngine;

namespace Puzzle.InEnergy.WeighBeaker
{
    public enum BottleType
    {
        Four,
        Six,
        Nine
    }

    public class Bottle : MonoBehaviour
    {
        public BottleType Type;

        public Collider2D col;
        public bool IsOnBalance = false;
        
        Vector3 m_Offset;
        Vector3 m_TargetScreenVec;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        private IEnumerator OnMouseDown()
        {
            col.enabled = false;
            Puzzle.Instance.HoldingBottle = this;

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

        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"{other.name}");
        }

        private void OnMouseUp()
        {
            transform.localPosition = Vector3.zero;
            col.enabled = true;

            Puzzle.Instance.HoldingBottle = null;
        }
    }
}