using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractiveNPC : InteractiveObject
{
    private int currentConversationCount;
    private NPCSentencesData NPCData;
    private BubbleController bubbleController;
    private CameraFollowController cameraFollowController;

    private bool AnsweredOrNot = false;
    private int answer = 0;

    public override void Deactivate()
    {
        if (bubbleController != null)
        {
            cameraFollowController.zoomSignal = 2;
            bubbleController.ClearAllBubbles();
            bubbleController = null;
        }
        base.Deactivate();
    }
    public override void LoadConfig()
    {
        NPCData = Resources.Load<NPCSentencesData>("ScriptableObjects/NPCSentencesData/NPC" + ID);
        cameraFollowController = GameObject.Find("Main Camera").GetComponent<CameraFollowController>();
        base.LoadConfig();
    }
    public override void TriggerEvent()
    {
        if (bubbleController == null)
        {
            cameraFollowController.zoomSignal = 1;
            bubbleController = new BubbleController(this.transform, NPCData, AnsweredOrNot, answer);
        }
        if (!bubbleController.conversationEnd) bubbleController.Next(); 
        else
        {
            cameraFollowController.zoomSignal = 2;
            bubbleController.ClearAllBubbles();
            bubbleController = null;
        }
        base.TriggerEvent();
    }
    public void setAnswer(int answer)
    {
        this.AnsweredOrNot = true;
        this.answer = answer;
    }
}

public class BubbleController
{
    private Sprite[] sprites;
    private Transform NPC;
    private SentenceType[] sentenceTypes;
    private int statement_length;
    private List<Bubble> Bubbles;
    private PlayerAnswerBubble playerAnswerBubble;

    private bool AnsweredOrNot = false;
    private bool needToAnswer = false;
    private bool CheckAnswer = false;
    private int answer = 0;
    private int count = 0;

    public bool conversationEnd = false;
    public NPCSentencesData NPCData;

    public BubbleController(Transform NPC, NPCSentencesData NPCData, bool AnsweredOrNot, int answer)
    {
        this.NPC = NPC;
        this.NPCData = NPCData;
        this.sprites = NPCData.SentencesData;
        this.sentenceTypes = NPCData.SentenceTypes;
        this.statement_length = NPCData.statement_length;
        this.needToAnswer = NPCData.needToAnswer;
        this.AnsweredOrNot = AnsweredOrNot;
        this.answer = answer;
        Bubbles = new List<Bubble>();
    }

    public void Next()
    {
        GameObject bubble;
        if (AnsweredOrNot && needToAnswer)
        {
            count = statement_length;
            CheckAnswer = true;
        }
        
        if (count < statement_length)
        {
            switch (sentenceTypes[count])
            {
                case SentenceType.NPC_Statement:
                    bubble = Resources.Load<GameObject>("Prefabs/NPCBubble/SingleBubble");
                    bubble = GameObject.Instantiate(bubble, NPC);
                    bubble.transform.Find("content").GetComponent<SpriteRenderer>().sprite = sprites[count];
                    Bubbles.Add(bubble.GetComponent<Bubble>());
                    foreach (Bubble _bubble in Bubbles)
                    {
                        _bubble.Next();
                    }
                    break;
                case SentenceType.Player_Answer:
                    if (playerAnswerBubble != null)
                    {
                        playerAnswerBubble.StartDisappear();
                        playerAnswerBubble = null;
                    }
                    GameObject playerbubble = Resources.Load<GameObject>("Prefabs/NPCBubble/PlayerAnswerBubble");
                    playerbubble = GameObject.Instantiate(playerbubble, GameObject.Find("TempPlayer").transform);
                    playerbubble.transform.Find("content").GetComponent<SpriteRenderer>().sprite = sprites[count];
                    playerAnswerBubble = playerbubble.GetComponent<PlayerAnswerBubble>();
                    playerAnswerBubble.setBubbleController(this);
                    break;
                case SentenceType.NPC_Answer:
                    break;
                default:
                    break;
            }
        }
        else
        {
            if (CheckAnswer)
            {
                bubble = Resources.Load<GameObject>("Prefabs/NPCBubble/SingleBubble");
                bubble = GameObject.Instantiate(bubble, NPC);
                bubble.transform.Find("content").GetComponent<SpriteRenderer>().sprite = sprites[count + answer];
                Bubbles.Add(bubble.GetComponent<Bubble>());
                foreach (Bubble _bubble in Bubbles)
                {
                    _bubble.Next();
                }
                NPC.GetComponent<InteractiveNPC>().setAnswer(answer);
            }
            conversationEnd = true;
        }
        if (count >= sprites.Length || (!CheckAnswer && count >= statement_length)) conversationEnd = true;
        if (Bubbles.Count > 2)
        {
            Bubbles[0].StartDisappear();
            Bubbles.RemoveAt(0);
        }
        count += 1;
    }

    public void ClearAllBubbles()
    {
        if (playerAnswerBubble != null)
        {
            playerAnswerBubble.StartDisappear();
            playerAnswerBubble = null;
        }
        while (Bubbles.Count != 0)
        {
            Bubble bubble = Bubbles[0];
            bubble.StartDisappear();
            Bubbles.Remove(bubble);
        }
    }
    public void setAnswer(bool val, int i)
    {
        CheckAnswer = val;
        answer = i;
    }
}