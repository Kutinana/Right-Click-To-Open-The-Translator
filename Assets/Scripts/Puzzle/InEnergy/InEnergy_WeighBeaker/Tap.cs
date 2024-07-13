using System.Collections;
using System.Collections.Generic;
using Cameras;
using DataSystem;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.WeighBeaker
{
    public class Tap : MonoBehaviour
    {
        public Collider2D col;
        public Image image;
        public List<Sprite> States;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
            image = GetComponent<Image>();
        }

        public void ChangeState(bool isOpen)
        {
            image.sprite = States[isOpen ? 1 : 0];
        }

        private void OnMouseOver()
        {
            if (Puzzle.Instance.HoldingBottle == null) return;
            Puzzle.Instance.Target = InteractTarget.Tap;
        }

        private void OnMouseExit()
        {
            if (Puzzle.Instance.HoldingBottle == null) return;
            Puzzle.Instance.Target = InteractTarget.None;
        }
    }
}