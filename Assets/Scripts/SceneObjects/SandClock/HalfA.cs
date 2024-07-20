using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SceneObject
{
    public class HalfA : MonoBehaviour
    {
        Quaternion startrot;
        private void Awake()
        {
            startrot = transform.rotation;
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
            float det = 1f / (20f / Time.fixedDeltaTime);
            float n = det;
            while (transform.rotation.eulerAngles.x < 87f)
            {
                float angle = n++ * det;
                var res = angle * angle - 2 * angle;
                transform.rotation = Quaternion.Euler(-1 * 90 * res, 0, 0);
                yield return new WaitForFixedUpdate();
            }
            gameObject.SetActive(false);
        }
        public void Initialize()
        {
            transform.rotation = startrot;
        }
    }
}
