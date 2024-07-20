using System.Collections;
using System.Collections.Generic;
using QFramework;
using UnityEngine;
using UnityEngine.Video;
namespace SceneObject
{
    public class HalfB : MonoBehaviour
    {
        Vector3 startpos;
        private void Awake()
        {
            startpos = transform.localPosition;
        }
        private void Start()
        {
            Flow();
        }
        public void Flow()
        {
            StopAllCoroutines();
            StartCoroutine(IEFlow());
        }
        IEnumerator IEFlow()
        {
            float det = Mathf.Abs(startpos.y) / (16f / Time.fixedDeltaTime);
            while (transform.localPosition.y < 0)
            {
                transform.localPosition = new Vector3(
                    transform.localPosition.x,
                    transform.localPosition.y + det,
                    transform.localPosition.z);
                yield return new WaitForFixedUpdate();
            }
            transform.localPosition = new Vector3(0, 0, 0);
        }
        public void Initialize()
        {
            transform.localPosition = startpos;
        }
    }
}
