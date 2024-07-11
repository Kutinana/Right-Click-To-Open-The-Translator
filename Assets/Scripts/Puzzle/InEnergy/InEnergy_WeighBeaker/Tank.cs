using System.Collections;
using System.Collections.Generic;
using Cameras;
using DataSystem;
using QFramework;
using UnityEngine;

namespace Puzzle.InEnergy.WeighBeaker
{
    public class Tank : MonoBehaviour
    {
        public Collider2D col;
        
        Vector3 m_Offset;
        Vector3 m_TargetScreenVec;

        private void Awake()
        {
            col = GetComponent<Collider2D>();
        }

        private IEnumerator OnMouseOver()
        {
            if (Puzzle.Instance.HoldingBottle == null) yield break;
            Puzzle.Instance.Target = InteractTarget.Tank;
        }

        private void OnMouseExit()
        {
            if (Puzzle.Instance.HoldingBottle == null) return;
            Puzzle.Instance.Target = InteractTarget.None;
        }
    }
}