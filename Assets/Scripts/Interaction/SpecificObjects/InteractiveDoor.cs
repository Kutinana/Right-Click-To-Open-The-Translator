
using UnityEngine;
using UnityEngine.Events;

public class InteractiveDoor: InteractiveObject
{
    public UnityEvent OnT;
    public override void TriggerEvent()
    {
        OnT.Invoke();
        base.TriggerEvent();
    }
    public void SwitchRoom(GameObject room)
    {
        room.SetActive(true);
    }
    public void LeaveRoom(GameObject room)
    {
        GameObject.Find("TempPlayer").GetComponent<PlayerController>().EnableGroundCheck = false;
        room.SetActive(false);
    }
    public void SetCameraMinX(int range)
    {
        CameraFollowController camera = GameObject.Find("Main Camera").GetComponent<CameraFollowController>();
        camera.minX = range;
    }
    public void SetCameraMaxX(int range)
    {
        CameraFollowController camera = GameObject.Find("Main Camera").GetComponent<CameraFollowController>();
        camera.maxX = range;
    }
}