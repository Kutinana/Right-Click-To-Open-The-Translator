using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;

namespace Puzzle.InCenter.Astrolable
{
    public class Fire : MonoBehaviour
    {
        private void OnMouseUp()
        {
            GameProgressData.IncreaseInventory("fire");
            gameObject.SetActive(false);
        }
    }
}