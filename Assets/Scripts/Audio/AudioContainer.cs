using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class AudioContainer : MonoBehaviour
{
    public AudioClip ambient;
    public AudioClip[] audioClips;
    public Dictionary<string,AudioClip> keyValuePairs;
    private void Awake() {
        foreach(AudioClip audioClip in audioClips){
            if(!audioClip.IsUnityNull()){
                this.keyValuePairs.Add(audioClip.name,audioClip);
            }
        }
    }
}
