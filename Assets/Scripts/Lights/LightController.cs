using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using QFramework;
using SceneControl;
using System;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.Rendering.Universal;
using System.Linq.Expressions;
namespace LightController
{
    public class LightController : MonoSingleton<LightController>
    {
        private GameObject[] mlights = new GameObject[5];
        short ptrLights = 0;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            TypeEventSystem.Global.Register<OnSceneLoadedEvent>(e =>
            {
                Debug.Log("Scene Loaded");
                OnSceneUpdate();
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
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
            foreach (GameObject g in mlights)
            {
                Debug.Log(g);
            }
            UpdateLights();
        }

        public void UpdateLights()
        {
            GameObject activeOne = null;
            for (int i = 0; i < 5; i++)
            {
                if(mlights[i].IsUnityNull())
                    break;
                mlights[i].transform.Find("Ambient").GetComponent<Light2D>().lightType = Light2D.LightType.Sprite;
                if(mlights[i].activeInHierarchy){
                    activeOne = mlights[i];
                }
            }
            try{
            activeOne.transform.Find("Ambient").GetComponent<Light2D>().lightType = Light2D.LightType.Global;
            }catch(Exception e){
                throw new Exception("Illegal Light Setting in "+SceneManager.GetActiveScene().name+":\n",e);
            }
        }
    }
}
