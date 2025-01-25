using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using QFramework;
using UnityEditor;
using UnityEngine;

namespace Steamworks
{
    public enum Achievement : int
    {
        ACH_FILLED_ALL_CREATORS,
        ACH_HANOI_ON_RIGHT,
        ACH_DOWN_THE_OCTOCAT_HOLE,
        ACH_SLIPPERY_HINT,
        ACH_BRAVE_NEW_WORLD,
        ACH_ARTISTS_FRIEND,
        ACH_THE_CREATION_OF_ADAM,
        ACH_ALFRED_NOBEL,
        ACH_CLEVERER_THAN_A_CHIMPANZEE,
        ACH_BHAGAVAD_GITA,
        ACH_TRUE_ENDS_REQUIREMENT,
        ACH_SCHOLAR,
        ACH_DEVELOPER_CERTIFICATED
    }

    public struct AchievementData
    {
        public Achievement achievement;
        public string name;
        public string description;
    }

    public struct OnAchievementUnlocked
    {
        public Achievement achievement;
    }

    public class SteamAchievementManager : MonoSingleton<SteamAchievementManager>
    {
        private CGameID m_GameID;

        private bool m_bRequestedStats;
        private bool m_bStatsValid;

        private Dictionary<Achievement, AchievementData> m_Achievements = new();

        protected Callback<UserStatsReceived_t> m_UserStatsReceived;
        protected Callback<UserStatsStored_t> m_UserStatsStored;
        protected Callback<UserAchievementStored_t> m_UserAchievementStored;

        private Coroutine StoreStatsCoroutine = null;

        public string AchievementNameToBeUnlocked { get; set; }

        private void Awake()
        {
            TypeEventSystem.Global.Register<OnAchievementUnlocked>(e => {
                ProcessAchievement(e.achievement);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEnable()
        {
            if (!SteamManager.Initialized) return;

            m_GameID = new CGameID(SteamUtils.GetAppID());

            m_UserStatsReceived = Callback<UserStatsReceived_t>.Create(OnUserStatsReceived);
            m_UserAchievementStored = Callback<UserAchievementStored_t>.Create(OnAchievementStored);

            // These need to be reset to get the stats upon an Assembly reload in the Editor.
            m_bRequestedStats = false;
            m_bStatsValid = false;

            if (!m_bRequestedStats) StartCoroutine(TryRequestUserStats());
        }

        internal void ProcessAchievement(Achievement achievement, bool activate = true)
        {
            if (!SteamManager.Initialized || !m_bStatsValid) return;

            if (activate) SteamUserStats.SetAchievement(achievement.ToString());
            else SteamUserStats.ClearAchievement(achievement.ToString());

            StoreStatsCoroutine ??= StartCoroutine(StoreStats());
        }

        private void OnUserStatsReceived(UserStatsReceived_t pCallback)
        {
            if (!SteamManager.Initialized) return;
            if ((ulong) m_GameID != pCallback.m_nGameID) return;

            if (EResult.k_EResultOK == pCallback.m_eResult)
            {
                m_bStatsValid = true;
                foreach (var ach in Enum.GetValues(typeof(Achievement)).Cast<Achievement>())
                {
                    if (SteamUserStats.GetAchievement(ach.ToString(), out _))
                    {
                        m_Achievements.Add(ach, new AchievementData() {
                            achievement = ach,
                            name = SteamUserStats.GetAchievementDisplayAttribute(ach.ToString(), "name"),
                            description = SteamUserStats.GetAchievementDisplayAttribute(ach.ToString(), "desc")
                        });
                    }
                }

                Debug.Log("User Stats Received");
            }
        }

        private void OnAchievementStored(UserAchievementStored_t pCallback)
        {
            if (!SteamManager.Initialized) return;
            if ((ulong) m_GameID != pCallback.m_nGameID) return;

            if (0 == pCallback.m_nMaxProgress)
            {
                Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' unlocked!");
            }
            else
            {
                Debug.Log("Achievement '" + pCallback.m_rgchAchievementName + "' progress callback, (" + pCallback.m_nCurProgress + "," + pCallback.m_nMaxProgress + ")");
            }
        }

        public void ClearAchievements()
        {
            if (!SteamManager.Initialized || !m_bStatsValid) return;

            foreach (var ach in Enum.GetValues(typeof(Achievement)).Cast<Achievement>())
            {
                if (SteamUserStats.GetAchievement(ach.ToString(), out var pbAchieved))
                {
                    if (pbAchieved) SteamUserStats.ClearAchievement(ach.ToString());
                }
            }
            StoreStatsCoroutine ??= StartCoroutine(StoreStats());
        }

        private IEnumerator TryRequestUserStats()
        {
            if (!SteamManager.Initialized) yield break;

            while (!SteamUserStats.RequestCurrentStats())
            {
                yield return new WaitForSeconds(1f);
            }
            m_bRequestedStats = true;
        }

        private IEnumerator StoreStats()
        {
            if (!SteamManager.Initialized || !m_bStatsValid) yield break;

            while (!SteamUserStats.StoreStats())
            {
                yield return new WaitForSeconds(1f);
            }
            Debug.Log("Stats Stored");
            StoreStatsCoroutine = null;
        }
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(SteamAchievementManager))]
    [CanEditMultipleObjects]
    public class SteamAchievementManagerEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            SteamAchievementManager manager = (SteamAchievementManager)target;

            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            manager.AchievementNameToBeUnlocked = EditorGUILayout.TextField(manager.AchievementNameToBeUnlocked);
            if (GUILayout.Button("Unlock"))
            {
                SteamAchievementManager.Instance.ProcessAchievement(Enum.Parse<Achievement>(manager.AchievementNameToBeUnlocked));
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("Clear All"))
            {
                SteamAchievementManager.Instance.ClearAchievements();
            }
        }
    }

#endif

}