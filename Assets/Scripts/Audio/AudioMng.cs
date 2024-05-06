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
    [SerializeField] float fadeTime = 60;
    private Dictionary<string, AudioClip> backGroundMusics = new();

    private float backgroundVolume;
    private float ambientVolume = 0.8f;
    private float effectVolume;

    ResLoader res;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        ResKit.Init();

        ambientChannel = transform.Find("Ambient").GetComponent<AudioSource>();
        BGM1 = transform.Find("Music1").GetComponent<AudioSource>();
        BGM2 = transform.Find("Music2").GetComponent<AudioSource>();
        current = BGM1;

        UpdateVolume();

        TypeEventSystem.Global.Register<OnVolumeSettingsChanged>(e => UpdateVolume()).UnRegisterWhenGameObjectDestroyed(gameObject);

        res = ResLoader.Allocate();
        if (PlayerPrefs.HasKey("Played") && PlayerPrefs.GetInt("Played") == 1)
        {
            AudioClip audioClip = res.LoadSync<AudioClip>("AmbientResearcher");
            ambientChannel.clip = audioClip;
        }
        else
        {
            AudioClip audioClip = res.LoadSync<AudioClip>("AmbientRain");
            ambientChannel.clip = audioClip;
        }
    }

    private void UpdateVolume()
    {
        backgroundVolume = PlayerPrefs.HasKey("Background Volume") ? PlayerPrefs.GetFloat("Background Volume") : 0.8f;
        if (current != null) current.volume = backgroundVolume;
        effectVolume = PlayerPrefs.HasKey("Effect Volume") ? PlayerPrefs.GetFloat("Effect Volume") : 0.8f;
        ambientVolume = PlayerPrefs.HasKey("Ambient Volume") ? PlayerPrefs.GetFloat("Ambient Volume") : 1.0f;
    }

    public void PlayAmbient()
    {
        ambientChannel.loop = true;
        ambientChannel.volume = ambientVolume;
        ambientChannel.Play();
    }
    public void PlayAmbient(string name)
    {
        if (name != ambientChannel.clip.name)
        {
            AudioClip audioClip = res.LoadSync<AudioClip>(name);
            ambientChannel.clip = audioClip;
        }
        PlayAmbient();
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
    public static void PlayBtnPressed(int type, float volumeScale = 1f)
    {
        if (type == 0)
        {
            AudioKit.PlaySound("click", volumeScale: Instance.effectVolume * volumeScale);
        }
        else if (type == 1)
        {
            AudioKit.PlaySound("apply", volumeScale: Instance.effectVolume * volumeScale);
        }
        else if (type == 2)
        {
            AudioKit.PlaySound("cancel", volumeScale: Instance.effectVolume * volumeScale);
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

    public static void StopBGM()
    {
        Instance.FadeMusic(Instance.current, 0f);
    }
    public static void StopAmbient()
    {
        Instance.FadeMusic(Instance.ambientChannel, 0f);
    }

    public static void StopAll()
    {
        Instance.FadeMusic(Instance.current, 0f);
        Instance.FadeMusic(Instance.ambientChannel, 0f);
    }

    public void FadeMusic(AudioSource audioSource, float target)
    {
        if (audioSource == null) return;

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

            AudioClip audioClip = res.LoadSync<AudioClip>(name);
            // AudioClip audioClip = Resources.Load<AudioClip>("Audios/BGM/" + name);
            backGroundMusics.Add(name, audioClip);
        }
    }
    public void ChangeAmbient(string name)
    {
        StartCoroutine(IEChangeAmbient(name));
    }
    public void ChangeBGM(string name)
    {
        AudioSource temp = GetAuxChannel();
        temp.volume = 0;
        current = temp;
        D_PlayBGM(name);
        FadeMusic(temp, backgroundVolume);
        FadeMusic(GetAuxChannel(), 0);
    }

    IEnumerator IEChangeAmbient(string name)
    {
        AudioSource temp = GetAuxChannel();
        temp.volume = 0;
        temp.clip = ambientChannel.clip;
        temp.Play();
        FadeMusic(temp, ambientVolume);
        FadeMusic(ambientChannel, 0);

        AudioClip newAmbient = res.LoadSync<AudioClip>(name);

        yield return new WaitUntil(() => ambientChannel.volume == 0);

        ambientChannel.clip = newAmbient;
        ambientChannel.Play();
        FadeMusic(temp, 0);
        FadeMusic(ambientChannel, ambientVolume);
    }

    private AudioSource GetAuxChannel()
    {
        return current == BGM1 ? BGM2 : BGM1;
    }
}
