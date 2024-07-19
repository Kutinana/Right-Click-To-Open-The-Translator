using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using Kuchinashi;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.ExplosiveSynthesizer
{
    public class Bomb : MonoBehaviour
    {
        private Image image;
        private Collider2D col;

        private void Awake()
        {
            image = GetComponent<Image>();
            col = GetComponent<Collider2D>();
        }

        private void OnMouseUp()
        {
            GameProgressData.IncreaseInventory("bomb", 1);
            gameObject.SetActive(false);
        }
    }
}
