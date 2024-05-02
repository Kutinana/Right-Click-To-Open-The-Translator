using System;
using System.Collections;
using System.Collections.Generic;
using QFramework;
using Settings;
using UnityEngine;

public class AudioMng : MonoSingleton<AudioMng>
{
    private readonly char[] randTab = new char[] { '0', '2', '1', '1', '0', '1', '3', '2', '1', '1', '3', '0', '3', '0', '2' };
    private int pRandTab = 0;
    AudioSource ambientChannel;
    AudioSource current;
    AudioSource BGM1;
    AudioSource BGM2;
    [SerializeField] AudioClip ambientClip;
    [SerializeField] float fadeTime = 100;
    private Dictionary<string, AudioClip> backGroundMusics;

    private float backgroundVolume;
    private float effectVolume;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        ResKit.Init();

        ambientChannel = transform.Find("Ambient").GetComponent<AudioSource>();
        BGM1 = transform.Find("Music1").GetComponent<AudioSource>();
        BGM2 = transform.Find("Music2").GetComponent<AudioSource>();
        backGroundMusics = new Dictionary<string, AudioClip>();
        ambientChannel.clip = ambientClip;
        ambientChannel.Play();

        UpdateVolume();

        TypeEventSystem.Global.Register<OnVolumeSettingsChanged>(e => UpdateVolume()).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void Start()
    {
        ambientChannel.clip = ambientClip;
        ambientChannel.Play();
        current = BGM1;
    }

    private void UpdateVolume()
    {
        backgroundVolume = PlayerPrefs.HasKey("Background Volume") ? PlayerPrefs.GetFloat("Background Volume") : 0.8f;
        if (current != null) current.volume = backgroundVolume;

        effectVolume = PlayerPrefs.HasKey("Effect Volume") ? PlayerPrefs.GetFloat("Effect Volume") : 0.8f;
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
    public void PlayBtnPressed(int type, float volumeScale = 1f)
    {
        if (type == 0)
        {
            AudioKit.PlaySound("click", volumeScale: effectVolume * volumeScale);
        }
        else if (type == 1)
        {
            AudioKit.PlaySound("apply", volumeScale: effectVolume * volumeScale);
        }
        else if (type == 2)
        {
            AudioKit.PlaySound("cancel", volumeScale: effectVolume * volumeScale);
        }
    }

    public void D_PlayBGM(string name)
    {
        if (backGroundMusics.TryGetValue(name, out var audioClip))
        {
            current.clip = audioClip;
            current.Play();
        }
        else
        {
            LoadBGM(name);
            D_PlayBGM(name);
        }
    }

    public void StopBGM()
    {
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
            yield return new WaitForFixedUpdate();
        }
        audioSource.volume = target;
    }

    public void LoadBGM(string name)
    {
        if (backGroundMusics.ContainsKey(name))
        {
            Debug.Log("Duplicated loading music source.");
        }
        else
        {
            var res = ResLoader.Allocate();

            AudioClip audioClip = res.LoadSync<AudioClip>("audio", name);
            // AudioClip audioClip = Resources.Load<AudioClip>("Audios/BGM/" + name);
            backGroundMusics.Add(name, audioClip);
        }
    }
}
