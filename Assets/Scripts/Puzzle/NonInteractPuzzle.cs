using System.Collections;
using System.Collections.Generic;
using DataSystem;
using QFramework;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle
{
    public class NonInteractPuzzle : PuzzleBase
    {
        private Button backButton;

        public override void OnEnter()
        {
            base.OnEnter();

            backButton = transform.Find("Menu/Back").GetComponent<Button>();
            backButton.onClick.AddListener(() => {
                PuzzleManager.Exit();
            });
            
            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>())
            {
                ids.Add(c.data.Id);
            }
            // UserDictionary.Unlock(ids);
            UserDictionary.AddRelatedPuzzleAndSave(ids, Id);

            GameProgressData.Solve(this);
        }

        public override void OnExit()
        {
            base.OnExit();
        }
    }
}