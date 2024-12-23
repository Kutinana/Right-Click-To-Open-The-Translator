using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class SettingMenuController : MonoBehaviour
    {
        private Coroutine CurrentCoroutine = null;

        private void OnMouseEnter()
        {
            if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);

            CurrentCoroutine = StartCoroutine(OnMouseEnterCoroutine());
        }

        private void OnMouseExit()
        {
            if (CurrentCoroutine != null) StopCoroutine(CurrentCoroutine);

            CurrentCoroutine = StartCoroutine(OnMouseExitCoroutine());
        }

        private IEnumerator OnMouseEnterCoroutine()
        {
            Vector2 targetPos = new Vector2(-160, 0);
            while (!Mathf.Approximately(transform.localPosition.x, -160))
            {
                transform.localPosition = Vector2.Lerp(transform.localPosition, targetPos, Time.deltaTime * 10);
                yield return new WaitForFixedUpdate();
            }

            CurrentCoroutine = null;
        }

        private IEnumerator OnMouseExitCoroutine()
        {
            Vector2 targetPos = new Vector2(-360, 0);
            while (!Mathf.Approximately(transform.localPosition.x, -360))
            {
                transform.localPosition = Vector2.Lerp(transform.localPosition, targetPos, Time.deltaTime * 10);
                yield return new WaitForFixedUpdate();
            }

            CurrentCoroutine = null;
        }
    }
}
