using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Bases.Console
{
    public class ConsoleCommandController : MonoBehaviour
    {
        private TMP_Text text;
        private Button button;

        private string stackTrace = "";

        private void Awake()
        {
            text = GetComponent<TMP_Text>();
            button = GetComponent<Button>();

            button.onClick.AddListener(() => {
                GUIUtility.systemCopyBuffer = string.Format("{0}\n{1}", text.text, stackTrace);
            });
        }

        public void Initialize(string condition, string stackTrace = null, LogType type = LogType.Log)
        {
            if (type is LogType.Error or LogType.Exception)
            {
                text.text = $"<color=red>{condition}</color>";
                this.stackTrace = stackTrace;
            }
            else if (type is LogType.Warning)
            {
                text.text = $"<color=yellow>{condition}</color>";
                this.stackTrace = stackTrace;
            }
            else
            {
                text.text = $"{condition}";
            }
        }
    }
}
