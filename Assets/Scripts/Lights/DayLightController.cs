using System.Collections;
using QFramework;
using SceneControl;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using static UnityEngine.Rendering.Universal.Light2D;

public class DayLightController : MonoBehaviour
{
    //public static bool isDayTime;
    public static float dayLightTime = 0;
    protected static float timeNormal = 0;


    private Color[] lightColor = new Color[]{
        new Vector4(1,1,1,1),
        new Vector4(0,0,0,1),
        new Vector4(1,1,1,1),
        new Vector4(0,0,0,1)
    };

    private Light2D m_DayLight;

    private float transTime2Normal(float var0, out float var1)
    {
        var1 = 0.5f - Mathf.Cos(var0 * 3.14f) * 0.5f;
        return var1;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        m_DayLight = gameObject.GetComponent<Light2D>();
        TypeEventSystem.Global.Register<OnSceneControlDeactivatedEvent>(e => UpdateScene()).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void FixedUpdate()
    {
        if (dayLightTime < 60f)
        {
            m_DayLight.color = Color.Lerp(lightColor[0], lightColor[1], transTime2Normal(dayLightTime / 60, out timeNormal));
            dayLightTime += Time.fixedDeltaTime;
        }
        else if (dayLightTime < 80f)
        {
            m_DayLight.color = Color.Lerp(lightColor[1], lightColor[2], transTime2Normal((dayLightTime - 60) / 20, out timeNormal));
            dayLightTime += Time.fixedDeltaTime;
        }
        else if (dayLightTime < 120f)
        {
            m_DayLight.color = Color.Lerp(lightColor[2], lightColor[3], transTime2Normal((dayLightTime - 80) / 40, out timeNormal));
            dayLightTime += Time.fixedDeltaTime;
        }
        else if (dayLightTime < 160f)
        {
            m_DayLight.color = Color.Lerp(lightColor[3], lightColor[0], transTime2Normal((dayLightTime - 120) / 40, out timeNormal));
            dayLightTime += Time.fixedDeltaTime;
        }
        else
        {
            dayLightTime = 0;
        }
    }

    private void UpdateScene()
    {
        if (SceneManager.GetActiveScene().name.Equals("StartScene") || SceneManager.GetActiveScene().name.Equals("Zero") || SceneManager.GetActiveScene().name.Equals("First"))
        {
            m_DayLight.enabled = false;
        }
        else
        {
            m_DayLight.GetComponent<Light2D>().enabled = true;
        }
    }
}
