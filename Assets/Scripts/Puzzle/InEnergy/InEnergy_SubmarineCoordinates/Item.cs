using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;

namespace Puzzle.InEnergy.SubmarineCoordinates
{
    public class Item : MonoBehaviour
    {
        private void OnMouseUp()
        {
            GameProgressData.IncreaseInventory("dirt");
            gameObject.SetActive(false);
        }
    }
}