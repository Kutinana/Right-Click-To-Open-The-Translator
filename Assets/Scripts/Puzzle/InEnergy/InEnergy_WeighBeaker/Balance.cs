using System.Collections;
using System.Collections.Generic;
using Cameras;
using DataSystem;
using QFramework;
using UnityEngine;

namespace Puzzle.InEnergy.WeighBeaker
{
    public class Balance : MonoBehaviour
    {
        public Collider2D col;

        public List<GameObject> States;
        public int State = 2;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        public void ChangeState(int state)
        {
            State = state;
            for (int i = 0; i < States.Count; i++)
            {
                States[i].SetActive(i == state);
            }
        }

        private void OnMouseOver()
        {
            if (Puzzle.Instance.HoldingBottle == null) return;
            Puzzle.Instance.Target = InteractTarget.Balance;
        }

        private void OnMouseExit()
        {
            if (Puzzle.Instance.HoldingBottle == null) return;
            Puzzle.Instance.Target = InteractTarget.None;
        }
    }
}