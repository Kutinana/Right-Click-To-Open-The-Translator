using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using Kuchinashi;
using QFramework;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InEnergy.ExplosiveSynthesizer
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        private Collider2D inputCol;
        public List<string> Inputs;
        public readonly List<string> CORRECT = new List<string>() { "fire", "dirt", "dirt2" };

        public Light Light;

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            Instance = this;

            inputCol = GetComponent<Collider2D>();
        }

        private void OnMouseUp()
        {
            inputCol.enabled = false;

            Inputs.Add(InteractableObjectManager.Current.Data.Id);
            InteractableObjectManager.DropAndExit();

            Light.ChangeState(Inputs.Count);
        }

        public override void OnEnter()
        {
            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                Destroy(inputCol);
                Light.ChangeState(5);

                if (!GameProgressData.GetInventory().TryGetValue("bomb", out var value) || value == 0)
                {
                    transform.Find("Interactable/Bomb").gameObject.SetActive(true);
                }
            }
            else
            {
                CurrentCoroutine = StartCoroutine(CheckAnswerCoroutine());

                TypeEventSystem.Global.Register<OnItemUseEvent>(e => {
                    inputCol.enabled = true;
                }).UnRegisterWhenGameObjectDestroyed(gameObject);
            }

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                PuzzleManager.Exit();
            });

            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>())
            {
                ids.Add(c.data.Id);
            }
            UserDictionary.AddRelatedPuzzleAndSave(ids, Id);
            
            GameProgressData.Unlock(this);
        }

        public override void OnExit()
        {
            base.OnExit();

            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
                CurrentCoroutine = null;
            }
        }

        private IEnumerator CheckAnswerCoroutine()
        {
            while (true)
            {
                yield return new WaitUntil(() => Inputs.Count == 3);
                if (Inputs.Count == CORRECT.Count && Inputs.Except(CORRECT).Count() == 0 && CORRECT.Except(Inputs).Count() == 0)
                {
                    AudioKit.PlaySound("023CorrectRecipe");
                    Light.ChangeState(5);
                    AudioKit.PlaySound("023MachineRunning");
                    yield return new WaitForSeconds(2f);
                    transform.Find("Interactable/Bomb").gameObject.SetActive(true);

                    break;
                }
                else
                {
                    Inputs.Clear();
                    AudioKit.PlaySound("023WrongRecipe");
                    Light.ChangeState(4);
                    transform.Find("Interactable/Smoke").gameObject.SetActive(true);

                    yield return new WaitForSeconds(1f);
                    Light.ChangeState(0);
                    transform.Find("Interactable/Smoke").gameObject.SetActive(false);
                }
            }

            PuzzleManager.Solved(isClosing: false);

            CurrentCoroutine = null;
        }
    }
}
