
using QFramework;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

namespace Puzzle.InCenter.HanoiTower
{
   public class Tower: MonoBehaviour
    {
        public Stack ItemStack = new Stack(5);
        private void Awake()
        {

        }

        private IEnumerator OnMouseDown()
        {
            if (Puzzle.Instance.CurrentItem == null && ItemStack.Count != 0)
            {
                Puzzle.Instance.CurrentTower = this;
                Puzzle.Instance.CurrentItem = ItemStack.Peek() as Item;
                Puzzle.Instance.OriginTower = this;
                this.OverTower();
            }
            //Debug.Log(Puzzle.CurrentItem.transform.name);
            yield return null;
        }

        private void OnMouseEnter()
        {
            Puzzle.Instance.CurrentTower = this;
            if (Puzzle.Instance.CurrentItem != null)
            {
                this.OverTower();
            }
        }

        public bool SettleItem()
        {
            if (Puzzle.Instance.CurrentItem != null)
            {
                if (this.ItemStack.Count != 0)
                {
                    Item Top = ItemStack.Peek() as Item;
                    if (Top.size < Puzzle.Instance.CurrentItem.size) return false;
                }
                SetPosition();
                this.ItemStack.Push(Puzzle.Instance.CurrentItem);
                return true;
            }
            return false;
        }

        private void OverTower()
        {
            Puzzle.Instance.CurrentItem.transform.localPosition =
                new Vector3(this.transform.localPosition.x, Puzzle.HoverPosition, transform.localPosition.z);
        }

        private void SetPosition()
        {
            int layer = this.ItemStack.Count;
            Puzzle.Instance.CurrentItem.transform.localPosition =
                new Vector3(this.transform.localPosition.x, Puzzle.Instance.Layer[layer], transform.localPosition.z);
        }
    }
}