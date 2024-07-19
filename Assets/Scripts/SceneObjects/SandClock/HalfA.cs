using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace SceneObject
{
    public class HalfA : MonoBehaviour
    {
        Quaternion startrot;
        private void Awake() {
            startrot = transform.rotation;
        }
        private void Start() {
            Flow();
        }
        public void Flow(){
            StopAllCoroutines();
            StartCoroutine(IEFlow());
        }
        IEnumerator IEFlow()
        {
            float det = 90f / (20f / Time.fixedDeltaTime);
            while (!Mathf.Approximately(transform.eulerAngles.x, 90f))
            {
                transform.Rotate(new Vector3(det,0,0));
                yield return new WaitForFixedUpdate();
            }
            GetComponentInChildren<SpriteRenderer>().enabled = false;
        }
        public void Initialize(){
            transform.rotation = startrot;
            GetComponentInChildren<SpriteRenderer>().enabled = true;
        }
    }
}
