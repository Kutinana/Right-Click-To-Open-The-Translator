using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;

namespace Puzzle.InCenter.HanoiTower
{
    public class HiddenButton : MonoBehaviour
    {
        private void OnMouseUp()
        {
            GameProgressData.IncreaseInventory("elevatorButton");
            gameObject.SetActive(false);
        }
    }
}