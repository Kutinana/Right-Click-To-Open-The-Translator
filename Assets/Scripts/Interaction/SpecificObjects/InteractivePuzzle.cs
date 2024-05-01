using Hint;
using Puzzle;

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
        
        base.TriggerEvent();
    }
}