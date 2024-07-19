
using QFramework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Puzzle.InCenter.HanoiTower
{
    public class Tower : MonoBehaviour
    {
        public Stack ItemStack = new Stack(5);
        private void Awake()
        {

        }

        private IEnumerator OnMouseDown()
        {
            if (Puzzle.CurrentItem == null && ItemStack.Count != 0)
            {
                Puzzle.CurrentTower = this;
                Puzzle.CurrentItem = ItemStack.Peek() as Item;
                Puzzle.OriginTower = this;
                AudioKit.PlaySound("BottleUp", volumeScale: 0.6f);
                this.OverTower();
            }
            //Debug.Log(Puzzle.CurrentItem.transform.name);
            yield return null;
        }

        private void OnMouseEnter()
        {
            Puzzle.CurrentTower = this;
            if (Puzzle.CurrentItem != null)
            {
                AudioKit.PlaySound("InteractClick", volumeScale: 0.2f);
                this.OverTower();
            }
        }

        public bool SettleItem()
        {
            if (Puzzle.CurrentItem != null)
            {
                if (this.ItemStack.Count != 0)
                {
                    Item Top = ItemStack.Peek() as Item;
                    if (Top.size < Puzzle.CurrentItem.size) return false;
                }
                AudioKit.PlaySound("R0", volumeScale: 0.5f);
                SetPosition();
                this.ItemStack.Push(Puzzle.CurrentItem);
                return true;
            }
            return false;
        }

        private void OverTower()
        {
            Puzzle.CurrentItem.transform.localPosition = new Vector3(this.transform.localPosition.x, Puzzle.HoverPosition, this.transform.localPosition.z);
        }

        private void SetPosition()
        {
            int layer = this.ItemStack.Count;
            Puzzle.CurrentItem.transform.localPosition = new Vector3(this.transform.localPosition.x, Puzzle.Layer[layer], this.transform.localPosition.z);
        }
    }
}