using System.Collections.Generic;
using UnityEngine;

public class Crossroad: MonoBehaviour
{
    public List<string> PreviousSceneName;
    public List<Vector2> CorrespondingPos;

    private void Awake()
    {
        foreach (var item in PreviousSceneName)
        {
            if (SceneControl.SceneControl.PreviousScene == item)
            {
                transform.Find("TempPlayer").localPosition = CorrespondingPos[PreviousSceneName.IndexOf(item)];
            }
        }
    }
}