using UnityEngine;
using QFramework;
using SceneControl;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;
namespace LightController
{
    public class LightController : MonoSingleton<LightController>
    {
        private GameObject[] mlights;

        void Awake()
        {
            DontDestroyOnLoad(gameObject);
            TypeEventSystem.Global.Register<OnSceneControlDeactivatedEvent>(e => OnSceneUpdate()).UnRegisterWhenGameObjectDestroyed(gameObject);
        }
        private void OnSceneUpdate()
        {
            mlights = GameObject.FindGameObjectsWithTag("Light");
            UpdateLights();
        }

        public void UpdateLights()
        {
            GameObject activeOne = null;
            for (int i = 0; i < mlights.Length; i++)
            {
                var temp = mlights[i].transform.Find("Ambient");
                if (temp == null) continue;
                temp.GetComponent<Light2D>().lightType = Light2D.LightType.Sprite;
                if (mlights[i].activeInHierarchy)
                {
                    activeOne = mlights[i];
                }
            }

            if (activeOne == null)
            {
                Debug.Log("No active lights");
                //场景中无启用灯光则认为使用日光照明(?)
                DayLightController.Instance.enableDayLight = true;
                return;
            }

            var tlight = activeOne.transform.Find("Ambient");
            if (tlight == null)
            {
                DayLightController.Instance.enableDayLight = true;
            }
            else
            {
                try
                {
                    tlight.GetComponent<Light2D>().lightType = Light2D.LightType.Global;
                    DayLightController.Instance.enableDayLight = false;
                }
                catch (Exception)
                {
                    Debug.LogError("Missing Light2D in " + tlight.ToString());
                }
            }
        }
    }
}
