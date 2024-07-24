namespace DataSystem
{
    public partial class GameProgressData
    {
        public static bool HaveReadNarration(string id) => Instance.Save.ReadNarrations.Contains(id);
        public static void CompleteNarration(string id)
        {
            if (Instance.Save.ReadNarrations.Contains(id)) return;

            Instance.Save.ReadNarrations.Add(id);
            Instance.Serialization();
        }
    }
}