using System.Collections;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TranslatorFilterController : MonoSingleton<TranslatorFilterController>
{
    Blit m_Blit;

    void Start()
    {
        gameObject.GetComponent<UnityEngine.Rendering.Volume>().profile.TryGet<Blit>(out m_Blit);
    }
    public void EnableFilter()
    {
        if(m_Blit.IsUnityNull()) return;
        m_Blit.Intensity.Override(0.45f);
    }
    public void DisableFilter()
    {
        if(m_Blit.IsUnityNull()) return;
        m_Blit.Intensity.Override(0.0f);
    }
}
