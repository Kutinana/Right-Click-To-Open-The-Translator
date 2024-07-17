using DataSystem;
using Kuchinashi;
using System.Collections;
using System.Collections.Generic;
using Translator;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Book.Sample
{
    public class Puzzle : PuzzleBase
    {
        public static Puzzle Instance;

        private Button backButton;

        private void Awake()
        {
            Instance = this;
        }

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
            UserDictionary.Unlock(ids);
        }
    }
}