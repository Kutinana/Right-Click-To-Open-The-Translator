using System.Collections;
using System.Collections.Generic;
using Kuchinashi.Utils.Progressable;
using UnityEngine;

namespace UI
{
    public class SettingMenuController : MonoBehaviour
    {
        public Progressable PositionProgressable;
        private Coroutine CurrentCoroutine = null;

        private void OnMouseEnter()
        {
            PositionProgressable.Lerp(0.05f);
        }

        private void OnMouseExit()
        {
            PositionProgressable.InverseLerp(0.05f);
        }
    }
}
