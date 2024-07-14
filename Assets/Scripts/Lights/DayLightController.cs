using QFramework;
using SceneControl;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class DayLightController : MonoBehaviour
{
    const float DAYINTENSITY = 1;
    const float NIGHTINTENSITY = 0.15f;
    public static float dayLightTime = 0;
    protected static float timeNormal = 0;

    public Color color1;
    public Color color2;
    public Color color3;
    public Color color4;
    public Color defultColor = new(1, 1, 1, 1);
    private Light2D m_DayLight;

    
    
    // Summary:
    //      生成非线性明暗曲线
    // 修改这个地方的函数可以改变明暗变化曲线，现在是一个二次函数
    // 传入一个0-1线性变化值，由下方fixedupdate获得
    // 输出一个0-1的浮点数表示两个阶段间的混合程度，用于插值。可以从DayLightController.timeNormal获取

    private float transTime2Normal(float var0, out float var1)
    {
        //var1 = Mathf.Clamp(0.5f - Mathf.Cos(var0 * 3.14f) * 0.5f, 0, 1);
        var1 = Mathf.Clamp(Mathf.Pow(var0, 2f), 0, 1);
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
            m_DayLight.intensity = Mathf.Lerp(DAYINTENSITY, NIGHTINTENSITY, transTime2Normal(dayLightTime / 60, out timeNormal));
            dayLightTime += Time.fixedDeltaTime;
        }
        else if (dayLightTime < 80f)
        {
            m_DayLight.intensity = Mathf.Lerp(NIGHTINTENSITY, DAYINTENSITY, transTime2Normal((dayLightTime - 60) / 20, out timeNormal));
            dayLightTime += Time.fixedDeltaTime;
        }
        else if (dayLightTime < 120f)
        {
            m_DayLight.intensity = Mathf.Lerp(DAYINTENSITY, NIGHTINTENSITY, transTime2Normal((dayLightTime - 80) / 40, out timeNormal));
            dayLightTime += Time.fixedDeltaTime;
        }
        else if (dayLightTime < 160f)
        {
            m_DayLight.intensity = Mathf.Lerp(NIGHTINTENSITY, DAYINTENSITY, transTime2Normal((dayLightTime - 120) / 40, out timeNormal));
            dayLightTime += Time.fixedDeltaTime;
        }
        else
        {
            dayLightTime = 0;
        }
        m_DayLight.color = GetLightColor();
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

    [SerializeField] float time1 = 0.25f;
    [SerializeField] float time2 = 0.3f;
    [SerializeField] float time3 = 0.45f;
    [SerializeField] float time4 = 0.5f;
    [SerializeField] float time5 = 0.6f;
    private Color GetLightColor()
    {
        if (m_DayLight.intensity < time1)
        {
            return color1;
        }
        else if (m_DayLight.intensity < time2)
        {
            return Color.Lerp(color1, color2, (m_DayLight.intensity - time1) / (time2 - time1));
        }
        else if (m_DayLight.intensity < time3)
        {
            return Color.Lerp(color2, color3, (m_DayLight.intensity - time2) / (time3 - time2));
        }
        else if (m_DayLight.intensity < time4)
        {
            return Color.Lerp(color3, color4, (m_DayLight.intensity - time3) / (time4 - time3));
        }
        else if (m_DayLight.intensity < time5)
        {
            return Color.Lerp(color4, defultColor, (m_DayLight.intensity - time4) / (time5 - time4));
        }
        else
        {
            return defultColor;
        }
    }
}
