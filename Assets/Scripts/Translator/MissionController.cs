using System.Collections.Generic;
using System.Linq;
using DataSystem;
using JetBrains.Annotations;
using QFramework;
using TMPro;
using UI;
using UnityEngine;

public class MissionController : MonoBehaviour, ISingleton
{
    public static MissionController Instance => SingletonProperty<MissionController>.Instance;
    private TextMeshProUGUI m_title;
    private TextMeshProUGUI m_description;
    private string[] missionProgressList;
    private string DetailShowing = "NOTHING";

    public GameObject prefab;
    public void OnSingletonInit() { }
    public void ChangeDetailMissionInfo(MissionData missionData)
    {
        m_title.SetText(missionData.Name);
        m_description.SetText("\n" + missionData.Description);
        Instance.DetailShowing = missionData.Id;
    }
    private void Awake()
    {
        var tempDetail = transform.Find("Content/Detail");
        m_title = tempDetail.Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        m_description = tempDetail.Find("Description").gameObject.GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        this.GenerateMissionList();
    }


    public void GenerateMissionList()
    {
        missionProgressList = GameProgressData.GetProgressingMission();
        var parent = transform.Find("Content/Scroll View/Viewport/Content");
        for (int i = 0; i < parent.childCount; i++)
        {
            Destroy(parent.GetChild(i).gameObject);
        }
        for (int i = 0; i < missionProgressList.Length; i++)
        {
            Mission.Initialize(GameDesignData.GetMissionDataById(missionProgressList[i]), parent);
        };
    }

    public void Refresh()
    {
        GenerateMissionList();
        DetailShowing = "NOTHING";
        m_title.SetText(" ");
        m_description.SetText("\nNo Content Selected.");
    }
    public void CheckMissionAndRefresh()
    {
        GenerateMissionList();
        if (missionProgressList.Contains<string>(DetailShowing)) return;
        DetailShowing = "NOTHING";

        m_title.SetText(" ");
        m_description.SetText("\nNo Content Selected.");
    }
}
