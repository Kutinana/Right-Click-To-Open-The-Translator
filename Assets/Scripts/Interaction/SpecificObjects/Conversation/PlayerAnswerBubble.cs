
using QFramework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAnswerBubble: Bubble
{
    private BubbleController bubbleController;
    private Button button;
    private GameObject ThoughtsList;

    public void setBubbleController(BubbleController bubbleController) => this.bubbleController = bubbleController;
    public void OnButtonClick()
    {
        Debug.Log("hello");
        this.ThoughtsList = Resources.Load<GameObject>("Prefabs/NPCBubble/ThoughtsList1");
        this.ThoughtsList = GameObject.Instantiate(ThoughtsList, GameObject.Find("Map1").transform);
        GameObject.Find("TempPlayer").GetComponent<PlayerInput>().DisableInputActions();
        this.ThoughtsList.SetActive(true);
    }
    public void ChangeContent(GameObject thought)
    {
        GameObject.Find("TempPlayer").transform.Find("PlayerAnswerBubble(Clone)").transform.Find("content").GetComponent<SpriteRenderer>().sprite 
            = thought.transform.Find("content").GetComponent<Image>().sprite;
        thought.GetComponentInParent<Thoughts>().Exit();
    }
    public void Branch(int num)
    {
        PlayerAnswerBubble bubble = GameObject.Find("TempPlayer").transform.Find("PlayerAnswerBubble(Clone)").GetComponent<PlayerAnswerBubble>();
        bubble.bubbleController.setAnswer(true, num);
    }
}
