using Hint;
using Puzzle;
using UnityEngine;

public class InteractivePuzzle: InteractiveObject
{
    private ItemType itemType;
    public override void LoadConfig()
    {
        base.LoadConfig();
        itemType = itemConfig.itemType;
    }
    public override void TriggerEvent()
    {
        switch (itemType)
        {
            case ItemType.DOOR:
                break;
            case ItemType.PUZZLE:
                PuzzleManager.Initialize(itemConfig.target_string);
                break;
            case ItemType.NPC:
                break;
            case ItemType.Hint:
                HintManager.Initialize(itemConfig.target_string);
                break;
            default:
                break;
        }
        //GameObject.Find("TempPlayer").GetComponent<PlayerInput>().DisableInputActions();
        //GameObject.Find("TempPlayer").GetComponent<Rigidbody2D>().simulated = false;
        base.TriggerEvent();
    }
    public override void EndTrigger()
    {
        GameObject.Find("TempPlayer").GetComponent<Rigidbody2D>().simulated = true;
        base.EndTrigger();
    }
}