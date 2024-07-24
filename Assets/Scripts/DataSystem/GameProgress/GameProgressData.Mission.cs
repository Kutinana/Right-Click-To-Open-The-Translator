using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DataSystem
{
    public partial class GameProgressData
    {
        public static string[] GetProgressingMission()
        {
            List<string> strings = new();
            string[] keys = Instance.Save.MissionProgress.Keys.ToArray<string>();
            foreach(string key in keys){
                Instance.Save.MissionProgress.TryGetValue(key,out var temp);
                if(temp==MissionProgress.Progressing){
                    strings.Add(key);
                }
            }
            return strings.ToArray();
        }

        public static MissionProgress? GetMissionProgress(string _id){
            return Instance.Save.MissionProgress.ContainsKey(_id) ? Instance.Save.MissionProgress[_id] : null;
        }

        public static void AddMission(string Id,bool showText = true){
            if (Instance.Save.MissionProgress.ContainsKey(Id)) return;
            
            Instance.Save.MissionProgress.Add(Id,MissionProgress.Progressing);
            Instance.Serialization();

            //!!这个地方原则上应该用Event, 但暂且先这样实现. 之后需要重构这句来解耦合
            if(showText) PersistentUIController.Instance.MissionHintShow("<material=\"fusion-pixel-missionMat\">新目标："+GameDesignData.GetMissionDataById(Id).Name);
        }

        public static void CompleteMission(string Id, bool showText = true){
            if (Instance.Save.MissionProgress.ContainsKey(Id))
            {
                Instance.Save.MissionProgress[Id] = MissionProgress.Completed;
            }
            else
            {
                Instance.Save.MissionProgress.Add(Id, MissionProgress.Completed);
            }
            Instance.Serialization();
            if(showText) PersistentUIController.Instance.MissionHintShow("<material=\"fusion-pixel-missionMat\">"+GameDesignData.GetMissionDataById(Id).Name+"：已完成！");
        }
    }
}