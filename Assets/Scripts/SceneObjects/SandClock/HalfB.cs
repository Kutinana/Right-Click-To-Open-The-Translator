using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
namespace SceneObject
{
    public class HalfB : MonoBehaviour
    {
        Vector3 startpos;
        private void Awake()
        {
            startpos = transform.localPosition;
        }
        public void Flow()
        {
            StopAllCoroutines();
            StartCoroutine(IEFlow());
        }
        IEnumerator IEFlow()
        {
            float det = Mathf.Abs(startpos.y) / (20f / Time.fixedDeltaTime);
            while (!Mathf.Approximately(transform.localPosition.y, 0))
            {
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    transform.localPosition.y + det,
                    transform.localPosition.z);
                yield return new WaitForFixedUpdate();
            }
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        public void Initialize()
        {
            transform.localPosition = startpos;
            GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
    }
}
