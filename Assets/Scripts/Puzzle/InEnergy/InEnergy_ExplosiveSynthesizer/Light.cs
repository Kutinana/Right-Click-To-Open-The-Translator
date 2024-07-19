using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using Kuchinashi;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.ExplosiveSynthesizer
{
    public class Light : MonoBehaviour
    {
        public List<Sprite> States;
        private Image image;

        private void Awake()
        {
            image = GetComponent<Image>();
        }

        public void ChangeState(int state)
        {
            AudioKit.PlaySound("Click", volumeScale: 0.1f);
            if (state >= States.Count) return;
            image.sprite = States[state];
        }
    }
}
