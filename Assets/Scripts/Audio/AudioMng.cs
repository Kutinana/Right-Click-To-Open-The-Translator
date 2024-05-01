using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public class AudioMng : MonoBehaviour
{
    public static AudioMng Instance;
    private readonly char[] randTab = new char[] { '0', '2', '1', '1', '0', '1', '3', '2', '1', '1', '3', '0', '3', '0', '2' };
    private int pRandTab = 0;
    AudioSource ambientChannel;
    AudioSource current;
    AudioSource BGM1;
    AudioSource BGM2;
    [SerializeField] AudioClip ambientClip;
    [SerializeField] float fadeTime = 100;
    private Dictionary<string, AudioClip> bcakGroundMusics;
    private void Awake()
    {
        if (Instance.IsUnityNull())
        {
            Instance = gameObject.GetComponent<AudioMng>();
            Instance.DontDestroyOnLoad();
        }
        else
        {
            Destroy(gameObject);
        }
        ambientChannel = transform.Find("Ambient").GetComponent<AudioSource>();
        BGM1 = transform.Find("Music1").GetComponent<AudioSource>();
        BGM2 = transform.Find("Music2").GetComponent<AudioSource>();
        bcakGroundMusics = new Dictionary<string, AudioClip>();
        ambientChannel.clip = ambientClip;
        ambientChannel.Play();
    }
    private void Start()
    {
        ambientChannel.clip = ambientClip;
        ambientChannel.Play();
        current = BGM1;
    }
    public void PlayFootsteps()
    {
        AudioKit.PlaySound("ftstp-2-" + randTab[pRandTab++]);
        if (pRandTab == 15) pRandTab = 0;
    }
    /// <summary>
    /// Play sfx when a UI button is pressed.
    /// 0: Click; 1: Apply; 2:Cancel
    /// </summary>
    /// <param name="type">the type of button that change the sfx it plays. 0: click; 1: Apply; 2:Cancel</param>
    public void PlayBtnPressed(int type)
    {
        if (type == 0)
        {
            AudioKit.PlaySound("click");
        }
        else if (type == 1)
        {
            AudioKit.PlaySound("apply");
        }
        else if (type == 2)
        {
            AudioKit.PlaySound("cancel");
        }
    }

    public void D_PlayBGM(string name)
    {
        if (bcakGroundMusics.ContainsKey(name))
        {
            AudioClip audioClip;
            bcakGroundMusics.TryGetValue(name, out audioClip);
            current.clip = audioClip;
            current.Play();
        }
        else
        {
            LoadBGM(name);
            D_PlayBGM(name);
        }
    }

    public void StopBGM(){
        FadeMusic(current,0f);
    }

    public void FadeMusic(AudioSource audioSource, float target)
    {
        float delV = (target - audioSource.volume) / fadeTime;
        StartCoroutine(FadeMusicTo(audioSource, target, delV));
    }
    IEnumerator FadeMusicTo(AudioSource audioSource, float target, float delV)
    {
        for (float i = audioSource.volume; i < target - 0.01 || i > target + 0.01; i += delV)
        {
            audioSource.volume = i;
            yield return 0;
        }
        audioSource.volume = target;
    }

    public void LoadBGM(string name)
    {
        if (bcakGroundMusics.ContainsKey(name))
        {
            Debug.Log("Duplicated loading music source.");
        }
        else
        {
            AudioClip audioClip = Resources.Load<AudioClip>("Audios/BGM/" + name);
            bcakGroundMusics.Add(name, audioClip);
        }
    }
}
