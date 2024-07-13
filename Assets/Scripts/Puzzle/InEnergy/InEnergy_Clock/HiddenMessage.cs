using System.Collections;
using System.Collections.Generic;
using Cameras;
using DataSystem;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.Clock
{
    public class HiddenMessage : MonoBehaviour
    {
        public Vector3 TargetPosition;

        public void ShowHiddenMessage()
        {
            StartCoroutine(ShowHiddenMessageCoroutine());
        }

        private IEnumerator ShowHiddenMessageCoroutine()
        {
            while (!Mathf.Approximately(transform.position.x, TargetPosition.x))
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, TargetPosition, 0.2f);
                yield return new WaitForFixedUpdate();
            }
            transform.localPosition = TargetPosition;
        }
    }
}