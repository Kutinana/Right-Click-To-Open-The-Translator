

using System.Collections;
using UnityEngine;

namespace Puzzle.InCenter.HanoiTower
{
    public class Item: MonoBehaviour
    {
        public Size size;
        public Vector3 InitialPosition;

        private void Awake()
        {
            //Debug.Log(transform.localPosition);
            InitialPosition = transform.localPosition;
        }
    }
}