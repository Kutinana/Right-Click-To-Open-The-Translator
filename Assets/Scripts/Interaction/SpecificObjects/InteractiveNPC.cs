using UnityEngine;

public class InteractiveNPC : InteractiveObject
{
    private int currentConversationCount;
    private NPCConfig NPC;
    private Sprite[] Sentences;
    public override void LoadConfig()
    {
        foreach (NPCConfig _NPCConfig in InteractiveObjectPool.Instance.NPCConfigs)
        {
            if (this.ID == _NPCConfig.ID) NPC = _NPCConfig;
        }
        if (NPC == null)
        {
            Debug.LogError("No corresponding NPC config. ID: " + ID);
        }
        else
        {
            Sentences = new Sprite[NPC.MaxConservationCount];
            for (int i = 0; i < Sentences.Length; i++)
            {
                Sentences[i] = Resources.Load<Sprite>(NPC.SentencesPath[i]) as Sprite;
            }
        }
        base.LoadConfig();
    }
    public override void TriggerEvent()
    {
        
        base.TriggerEvent();
    }
}