using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InCenter.Lighting
{
    public class CubeLighting : MonoBehaviour
    {
        Button m_button;
        LightState currentState;

        private void Awake()
        {
            m_button = GetComponent<Button>();

            m_button.onClick.AddListener(() => SwitchStateWithChain());
        }

        public void SwitchStateWithChain()
        {
            AudioKit.PlaySound("023ClickLight", volumeScale: 0.5f);
            if (currentState == LightState.Off) SetState(LightState.On);
            else SetState(LightState.Off);

            foreach (var light in Puzzle.LightChain[this])
            {
                light.SwitchStateWithoutChain();
            }
        }

        public void SwitchStateWithoutChain()
        {
            if (currentState == LightState.Off) SetState(LightState.On);
            else SetState(LightState.Off);
        }

        public void SetState(LightState lightState)
        {
            switch (lightState)
            {
                case LightState.On:
                    transform.GetComponent<Image>().sprite = Puzzle.Instance.LightSprite[0];
                    break;
                case LightState.Off:
                    transform.GetComponent<Image>().sprite = Puzzle.Instance.LightSprite[1];
                    break;
                default:
                    break;
            }
            currentState = lightState;
            Puzzle.CurrentStates[int.Parse(transform.name) - 1] = (int)currentState;
        }
    }
}