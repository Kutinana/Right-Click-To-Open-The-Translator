using QFramework;
using TMPro;
using UnityEngine;

public class PersistentUIController : MonoSingleton<PersistentUIController>
{
    private GameObject m_MissionHintUi;
    private void Awake() {
        m_MissionHintUi = transform.Find("MissionHintText").gameObject;
    }
    public void MissionHintShow(string content){
        m_MissionHintUi.GetComponent<TextMeshProUGUI>().SetText(content);
        m_MissionHintUi.GetComponent<Animation>().Play();
    }
}
