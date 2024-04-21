using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractiveNPC : InteractiveObject
{
    private int currentConversationCount;
    private NPCSentencesData NPCData;
    private Sprite[] Sentences;
    private BubbleController bubbleController;
    private CameraFollowController cameraFollowController;

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
        Sentences = NPCData.SentencesData;
        cameraFollowController = GameObject.Find("Main Camera").GetComponent<CameraFollowController>();
        base.LoadConfig();
    }
    public override void TriggerEvent()
    {
        if (bubbleController == null)
        {
            cameraFollowController.zoomSignal = 1;
            bubbleController = new BubbleController(this.transform, Sentences);
        }
        if (bubbleController.count <= Sentences.Length) bubbleController.Next(); 
        if (bubbleController.count > Sentences.Length)
        {
            cameraFollowController.zoomSignal = 2;
            bubbleController.ClearAllBubbles();
            bubbleController = null;
        }
        base.TriggerEvent();
    }
}

public class BubbleController
{
    private Sprite[] sprites;
    private Transform NPC;
    private List<Bubble> Bubbles;

    public int count = 0;

    public BubbleController(Transform NPC, Sprite[] sprites)
    {
        this.NPC = NPC;
        this.sprites = sprites;
        Bubbles = new List<Bubble>();
    }

    public void Next()
    {
        if (count < sprites.Length)
        {
            GameObject bubble = Resources.Load<GameObject>("Prefabs/NPCBubble/SingleBubble");
            bubble = GameObject.Instantiate(bubble, NPC);
            bubble.GetComponent<SpriteRenderer>().sprite = sprites[count];
            Bubbles.Add(bubble.GetComponent<Bubble>());
            foreach (Bubble _bubble in Bubbles)
            {
                _bubble.Next();
            }
        }
        if (Bubbles.Count > 2)
        {
            Bubbles[0].StartDisappear();
            Bubbles.RemoveAt(0);
        }
        count += 1;
    }

    public void ClearAllBubbles()
    {
        while (Bubbles.Count != 0)
        {
            Bubble bubble = Bubbles[0];
            bubble.StartDisappear();
            Bubbles.Remove(bubble);
        }
    }
}