using System.Collections;
using System.Collections.Generic;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public class AudioMng : MonoBehaviour
{
    private readonly char[] randTab = new char[] { '0', '2', '1', '1', '0', '1', '3', '2', '1', '1', '3', '0', '3', '0', '2' };
    private int pRandTab = 0;
    public static AudioMng Instance;
    public AudioMng()
    {

    }
    private void Awake()
    {
        if (Instance.IsUnityNull())
        {
            Instance = new AudioMng();
            gameObject.DontDestroyOnLoad();
        }
        else
        {
            Destroy(this);
        }
    }
    public void PlayFootsteps()
    {
        QFramework.AudioKit.PlaySound("ftstp-2-" + randTab[pRandTab++]);
        if (pRandTab == 15) pRandTab = 0;
    }
    public void PlayClick()
    {
        QFramework.AudioKit.PlaySound("click");
    }
}
