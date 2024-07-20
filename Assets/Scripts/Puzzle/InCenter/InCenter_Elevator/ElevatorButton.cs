using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UI;
using UnityEngine;
using UnityEngine.Events;

namespace Puzzle.InCenter.Elevator
{
    public class ElevatorButton : MonoBehaviour
    {
        private Collider2D col;

        public UnityEvent OnClick;
        public bool Interactable;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        private void OnMouseUp()
        {
            if (Interactable)
            {
                OnClick?.Invoke();
                foreach (var btn in transform.parent.GetComponentsInChildren<ElevatorButton>()) btn.Interactable = false;

                PuzzleManager.Exit(1f);
            }
            else if (InteractableObjectManager.Current != null && InteractableObjectManager.Current.Data.Id == "elevatorButton")
            {
                for (var i = 0; i < transform.childCount; i++) transform.GetChild(i).gameObject.SetActive(true);
                Interactable = true;

                InteractableObjectManager.Exit();

                PuzzleManager.Solved(isClosing: false);
            }
        }
    }
}