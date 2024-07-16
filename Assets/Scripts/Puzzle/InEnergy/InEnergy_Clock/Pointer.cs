using System.Collections;
using System.Collections.Generic;
using Cameras;
using DataSystem;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.Clock
{
    public class Pointer : MonoBehaviour
    {
        public Collider2D col;


        Vector3 m_Offset;
        Vector3 m_TargetScreenVec;

        public bool isInteractable = true;
        //private AudioPlayer audioPlayer;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        private IEnumerator OnMouseDown()
        {
            col.enabled = false;
            yield return new WaitUntil(() => this.isInteractable);
            AudioKit.PlaySound("BottleUp");
            //this.audioPlayer = AudioKit.PlaySound("ClockHands", true, volumeScale: 0.5f);
            m_TargetScreenVec = PuzzleCameraManager.Camera.WorldToScreenPoint(transform.position);
            m_Offset = TranslatorCameraManager.Camera.ScreenToViewportPoint(new Vector3
                (Input.mousePosition.x, Input.mousePosition.y, 1f)) - new Vector3(0.5f, 0.5f);

            var currentRotation = transform.localEulerAngles.z;

            while (Input.GetMouseButton(0))
            {
                Vector3 res = TranslatorCameraManager.Camera.ScreenToViewportPoint(new Vector3(Input.mousePosition.x,
                    Input.mousePosition.y, 1f)) - new Vector3(0.5f, 0.5f);

                transform.localEulerAngles = new Vector3(0, 0, -Vector2.SignedAngle(res, m_Offset) + currentRotation);

                currentRotation = transform.localEulerAngles.z;
                m_Offset = res;

                yield return new WaitForFixedUpdate();
            }
        }

        private void OnMouseUp()
        {
            if (!isInteractable) return;
            col.enabled = true;
            //this.audioPlayer.Stop();
        }
    }
}