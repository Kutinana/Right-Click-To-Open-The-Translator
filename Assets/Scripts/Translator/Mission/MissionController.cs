using System.Linq;
using DataSystem;
using JetBrains.Annotations;
using QFramework;
using TMPro;
using UI;
using UI.Narration;
using UnityEngine;
using UnityEngine.UI;

public class MissionController : MonoBehaviour, QFramework.ISingleton
{
    public static MissionController Instance => SingletonProperty<MissionController>.Instance;
    private TextMeshProUGUI m_title;
    private TextMeshProUGUI m_description;
    private string[] missionProgressList;
    public string DetailShowing = "NOTHING";
    private Image currentMissionImage;

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
        var tempDetail = transform.Find("Image/Content/Detail");
        m_title = tempDetail.Find("Title").gameObject.GetComponent<TextMeshProUGUI>();
        m_description = tempDetail.Find("Description").gameObject.GetComponent<TextMeshProUGUI>();
        TypeEventSystem.Global.Register<OnNarrationEndEvent>(e => OnNarrationEndMissionHandler(e)).UnRegisterWhenGameObjectDestroyed(gameObject);
    }

    private void Start()
    {
        this.GenerateMissionList();
    }


    public void GenerateMissionList()
    {
        missionProgressList = GameProgressData.GetProgressingMission();
        var parent = transform.Find("Image/Content/Scroll View/Viewport/Content");
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
        TypeEventSystem.Global.Send(new MissionListRefreshedEvent(DetailShowing));
        m_title.SetText(" ");
        m_description.SetText("\nNo Content Selected.");
    }
    private void OnNarrationEndMissionHandler(OnNarrationEndEvent e)
    {
        if (!string.IsNullOrEmpty(e.Id) && e.Id.Equals("InitialNarration"))
        {
            GameProgressData.AddMission("main0");
        }
    }

    public void SetHighlightImage(Image image)
    {
        if (!(currentMissionImage == null))
        {
            currentMissionImage.color = new Color(r: 0.7960785f, g: 0.91372555f, b: 0.8117648f, a: 1);
        }
        image.color = new Color(0.8865123f, 0.8943396f, 0.6867853f, 1);
        currentMissionImage = image;
    }
    public struct MissionListRefreshedEvent{
        public MissionListRefreshedEvent(string id){
            this.id = id;
        }
        public string id;
    }
}
