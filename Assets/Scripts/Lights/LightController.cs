using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using SceneControl;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;
namespace LightController
{
    public class LightController : MonoSingleton<LightController>
    {
        private GameObject[] mlights = new GameObject[5];
        short ptrLights = 0;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            TypeEventSystem.Global.Register<OnSceneControlDeactivatedEvent>(e => OnSceneUpdate()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        private void OnSceneUpdate()
        {
            ptrLights = 0;
            Array.Clear(mlights, 0, mlights.Length);
            foreach (GameObject gameObject in Resources.FindObjectsOfTypeAll(typeof(GameObject)))
            {
                if (ptrLights < 5 && gameObject.CompareTag("Light"))
                {
                    mlights[ptrLights++] = gameObject;
                }
            }
            UpdateLights();
        }

        public void UpdateLights()
        {
            GameObject activeOne = null;
            for (int i = 0; i < 5; i++)
            {
                if (mlights[i].IsUnityNull())
                    break;
                mlights[i].transform.Find("Ambient").GetComponent<Light2D>().lightType = Light2D.LightType.Sprite;
                if (mlights[i].activeInHierarchy)
                {
                    activeOne = mlights[i];
                }
            }

            if (activeOne == null) return;

            try
            {
                activeOne.transform.Find("Ambient").GetComponent<Light2D>().lightType = Light2D.LightType.Global;
            }
            catch (Exception)
            {
                Debug.Log("Illegal Light Setting in " + SceneManager.GetActiveScene().name + ":\n");
            }
        }
    }
}
