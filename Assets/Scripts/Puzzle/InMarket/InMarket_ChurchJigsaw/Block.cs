
using Puzzle.InEnergy.Cable;
using QFramework;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InMarket.Church
{
    public class Block : MonoBehaviour
    {
        BlockState currentState;
        Button button;
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => NextState());
        }

        private void AnalyzeState(BlockState blockState)
        {
            switch (blockState)
            {
                case BlockState.Up:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case BlockState.Left:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    break;
                case BlockState.Down:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    break;
                case BlockState.Right:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                    break;
                default:
                    break;
            }
        }

        public void SetState(BlockState blockState)
        {
            AnalyzeState(blockState);
            currentState = blockState;
        }

        private void NextState()
        {
            AudioKit.PlaySound("023ItemRot", volumeScale: .8f);
            int val = ((int)currentState + 1) % 4;
            BlockState nextState = (BlockState)val;
            SetState(nextState);
            Puzzle.SetState(nextState, this);
        }
    }
}