using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.InMarket.LiquidToDirtShop
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;
        private Button backButton;

        public HiddenTentacle HiddenTentacle;

        private void Awake()
        {
            Instance = this;

            HiddenTentacle = GetComponentInChildren<HiddenTentacle>(includeInactive: true);
        }

        public override void OnEnter()
        {
            base.OnEnter();

            if (GameProgressData.GetPuzzleProgress(Id) == PuzzleProgress.Solved)
            {
                HiddenTentacle.gameObject.SetActive(true);
                HiddenTentacle.transform.Find("Dirt").gameObject.SetActive(false);
                HiddenTentacle.transform.GetComponent<Collider2D>().enabled = false;
                HiddenTentacle.transform.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            }
            else
            {
                HiddenTentacle.gameObject.SetActive(false);
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
        }

        private void OnMouseUp()
        {
            if (InteractableObjectManager.IsHolding("yellowLiquid"))
            {
                AudioKit.PlaySound("PaintingMoving");
                GetComponent<Collider2D>().enabled = false;

                InteractableObjectManager.DropAndExit();
                HiddenTentacle.ShowHiddenTentacle();

                PuzzleManager.Solved(isClosing: false);
            }
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}
