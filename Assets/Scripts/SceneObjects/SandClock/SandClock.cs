using System.Collections;
using System.Net.Sockets;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;
namespace SceneObject
{
    public class SandClock : MonoBehaviour
    {
        public GameObject HalfA;
        public GameObject HalfB;
        Quaternion rot;
        int currentrot;
        public static Vector3 centerPoint;
        private void Awake()
        {
            rot = transform.rotation;
            centerPoint = transform.position;
            TypeEventSystem.Global.Register<DayLightController.TwenSecCountEvent>(e => Flip());
        }
        public void Flip()
        {
            StartCoroutine(IEFlip());
        }
        IEnumerator IEFlip()
        {
            while(!(currentrot==180))
            {
                transform.Rotate(new Vector3(0, 0, 5));
                currentrot += 5;
                yield return new WaitForFixedUpdate();
            }
            Initialize();
            HalfA.GetComponent<HalfA>().Flow();
            HalfB.GetComponent<HalfB>().Flow();
            currentrot = 0;
        }
        public void Initialize()
        {
            transform.SetPositionAndRotation(transform.position, rot);
            HalfA.GetComponent<HalfA>().Initialize();
            HalfB.GetComponent<HalfB>().Initialize();
        }
    }
}