using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.Cable
{
    public class Cable : MonoBehaviour
    {
        CableState currentState;
        Button button;
        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(() => NextState());
        }

        private void AnalyzeState(CableState cableState)
        {
            switch (cableState)
            {
                case CableState.Up:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                    break;
                case CableState.Right:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));
                    break;
                case CableState.Down:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
                    break;
                case CableState.Left:
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, 270));
                    break;
                default:
                    break;
            }
        }

        public void SetState(CableState cableState)
        {
            AnalyzeState(cableState);
            currentState = cableState;
        }

        private void NextState()
        {
            AudioKit.PlaySound("Cube-Slide");
            int val = ((int)currentState + 1) % 4;
            CableState nextState = (CableState)val;
            SetState(nextState);
            Puzzle.SetState(nextState, this);
        }
    }
}