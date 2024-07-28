using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UI;
using UnityEngine;

namespace Puzzle.InCenter.HanoiTower
{
    public class HiddenButton : MonoBehaviour
    {
        private void OnMouseUp()
        {
            GameProgressData.IncreaseInventory("elevatorButton");
            UserDictionary.AddRelatedPuzzleAndSave(GetComponentInChildren<Character>().data.Id, Puzzle.Instance.Id);

            gameObject.SetActive(false);
        }
    }
}