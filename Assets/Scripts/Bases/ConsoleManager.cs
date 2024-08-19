using System.Collections;
using System.Collections.Generic;
using Bases.Console;
using DataSystem;
using Puzzle;
using QFramework;
using Steamworks;
using TMPro;
using UnityEngine;

namespace Bases
{
    public class ConsoleManager : MonoSingleton<ConsoleManager>
    {
        private CanvasGroup canvasGroup;
        private TMP_InputField inputField;

        public bool IsActivated = false;

        public GameObject CommandPrefab;
        private Transform commandParent;

        private void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;

            inputField = transform.Find("InputField").GetComponent<TMP_InputField>();

            commandParent = transform.Find("Image/Scroll View/Viewport/Content");

            DontDestroyOnLoad(gameObject);
            Application.logMessageReceived += (condition, stackTrace, type) =>
            {
                Instantiate(CommandPrefab, commandParent).GetComponent<ConsoleCommandController>().Initialize(condition, stackTrace, type);
            };
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) && inputField.isActiveAndEnabled)
            {
                string command = inputField.text;
                inputField.text = "";
                if (!string.IsNullOrEmpty(command)) ProcessCommand(command);
            }
            else if (Input.GetKeyDown(KeyCode.Return) && !inputField.isActiveAndEnabled)
                inputField.ActivateInputField();

            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                canvasGroup.alpha = IsActivated ? 0 : 1;
                canvasGroup.interactable = IsActivated ? false : true;
                canvasGroup.blocksRaycasts = IsActivated ? false : true;

                if (IsActivated)
                {
                    inputField.text = "";
                    PlayerInput.Instance.EnableInputActions();
                }
                else
                {
                    inputField.ActivateInputField();
                    PlayerInput.Instance.DisableInputActions();
                }

                IsActivated = !IsActivated;
            }

            if (commandParent.childCount > 100)
            {
                ProcessCommand("clear");
            }
        }

        private void ProcessCommand(string command)
        {
            var commands = command.Split(" ");

            switch(commands[0])
            {
                case "clear":
                    foreach (Transform child in commandParent)
                    Destroy(child.gameObject);
                    return;
                case "add_inventory" when commands.Length == 2:
                    GameProgressData.TryIncreaseInventory(commands[1]);
                    Instantiate(CommandPrefab, commandParent).GetComponent<ConsoleCommandController>().Initialize($"Obtained {commands[1]}.");
                    return;
                case "add_inventory" when commands.Length == 3:
                    GameProgressData.TryIncreaseInventory(commands[1], int.Parse(commands[2]));
                    Instantiate(CommandPrefab, commandParent).GetComponent<ConsoleCommandController>().Initialize($"Obtained {commands[2]} {commands[1]}.");
                    return;
                case "puzzle" when commands.Length == 2:
                    PuzzleManager.Initialize(commands[1]);
                    Instantiate(CommandPrefab, commandParent).GetComponent<ConsoleCommandController>().Initialize($"Initialized {commands[1]}.");
                    return;
                case "tp" when commands.Length == 2:
                    if (SceneControl.SceneControl.TrySwitchSceneWithoutConfirm(commands[1]))
                    {
                        Instantiate(CommandPrefab, commandParent).GetComponent<ConsoleCommandController>().Initialize($"Teleported to {commands[1]}.");
                        return;
                    }
                    else
                    {
                        Instantiate(CommandPrefab, commandParent).GetComponent<ConsoleCommandController>().Initialize($"Unknown SceneName.", type: LogType.Error);
                        return;
                    }
                case "clear_ach":
                    SteamAchievementManager.Instance.ClearAchievements();
                    return;
                case "open_log":
                    Instantiate(CommandPrefab, commandParent).GetComponent<ConsoleCommandController>().Initialize("Reveal in explorer.");
                    Application.OpenURL(Application.persistentDataPath);
                    return;
            }

            Instantiate(CommandPrefab, commandParent).GetComponent<ConsoleCommandController>().Initialize("Unknown Command", type: LogType.Error);
        }
    }
}
