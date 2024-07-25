using System.Collections.Generic;
using UnityEngine;

public class Crossroad: MonoBehaviour
{
    public List<string> PreviousSceneName;
    public List<Vector2> CorrespondingPos;
    public Transform player;
    public CameraFollowController Camera;
    
    private void Awake()
    {
        Debug.Log(SceneControl.SceneControl.PreviousScene);
    }

    private void Start()
    {
        for (int i = 0; i < PreviousSceneName.Count; i++)
        {
            if (SceneControl.SceneControl.PreviousScene == PreviousSceneName[i])
            {
                Debug.Log("yes");
                player.position = new Vector3(CorrespondingPos[i].x, CorrespondingPos[i].y, player.position.z);
                Camera.transform.position = new Vector3(Mathf.Clamp(CorrespondingPos[i].x, Camera.minX, Camera.maxX) , Camera.transform.position.y, Camera.transform.position.z);
            }
        }
    }
}