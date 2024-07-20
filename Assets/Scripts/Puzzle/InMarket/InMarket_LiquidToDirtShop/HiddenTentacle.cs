using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;

namespace Puzzle.InMarket.LiquidToDirtShop
{
    public class HiddenTentacle : MonoBehaviour
    {
        public void ShowHiddenTentacle()
        {
            gameObject.SetActive(true);
            StartCoroutine(ShowHiddenTentacleCoroutine());
        }

        private IEnumerator ShowHiddenTentacleCoroutine()
        {
            var rect = GetComponent<RectTransform>();
            while (!Mathf.Approximately(rect.anchoredPosition.y, 0))
            {
                rect.anchoredPosition = Vector2.Lerp(rect.anchoredPosition, Vector2.zero, 0.2f);
                yield return new WaitForFixedUpdate();
            }
            rect.anchoredPosition = Vector2.zero;
        }

        private void OnMouseUp()
        {
            GameProgressData.IncreaseInventory("dirt2");
            transform.Find("Dirt").gameObject.SetActive(false);
        }
    }
}