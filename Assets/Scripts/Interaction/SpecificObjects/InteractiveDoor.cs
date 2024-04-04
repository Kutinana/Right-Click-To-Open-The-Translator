
public class InteractiveDoor: InteractiveObject
{
    string nextScene;
    public override void TriggerEvent()
    {
        nextScene = DoorConfig.nextSceneName[ID];
        //switch to next scene
        base.TriggerEvent();
    }
}