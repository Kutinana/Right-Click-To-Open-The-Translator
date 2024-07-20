using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Collider2D))]
public class BGMControl : MonoBehaviour
{
    public bool doStopBGM;
    public string bgmName;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (doStopBGM)
        {
            AudioMng.StopBGM();
            AudioMng.isPlayingName = null;
        }
        else
        {
            if ((AudioMng.isPlayingName != null) && AudioMng.isPlayingName.Equals(bgmName))
            {
                return;
            }
            AudioMng.isPlayingName = bgmName;
            AudioMng.Instance.PlayBGM(bgmName);
        }
        return;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (doStopBGM)
        {
            AudioMng.StopBGM();
            AudioMng.isPlayingName = null;
        }
        else
        {
            if ((AudioMng.isPlayingName != null) && AudioMng.isPlayingName.Equals(bgmName))
            {
                return;
            }
            AudioMng.isPlayingName = bgmName;
            AudioMng.Instance.PlayBGM(bgmName);
        }
        return;
    }
}
