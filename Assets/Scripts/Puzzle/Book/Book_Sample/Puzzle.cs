using DataSystem;
using Kuchinashi;
using QFramework;
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
        private List<Transform> Pages = new();
        static int pl = 0, p = 0;

        private void Awake()
        {
            Instance = this;
        }

        public override void OnEnter()
        {
            base.OnEnter();

            backButton = transform.Find("Menu/Back").GetComponent<Button>();

            foreach (Transform child in transform.Find("Characters"))
            {
                Pages.Add(child);
                pl++;
            }
            Cover();

            backButton.onClick.AddListener(() =>
            {
                PuzzleManager.Exit();
            });

            List<string> ids = new List<string>();
            foreach (var c in GetComponentsInChildren<Character>())
            {
                ids.Add(c.data.Id);
            }
            UserDictionary.Unlock(ids);
        }
        public void NextPage()
        {
            if (p == pl)
            {
                Cover();
            }
            else
            {
                Goto(p+1);
            }
        }
        private void Goto(int i)
        {
            if (i == 0)
            {
                Cover();
            }
            else
            {
                if (!(p == 0))
                {
                    Pages[p - 1].gameObject.SetActive(false);
                    Pages[p - 1].GetComponent<CanvasGroup>().alpha = 0;
                }
                Pages[i - 1].gameObject.SetActive(true);
                StartCoroutine(CanvasGroupHelper.FadeCanvasGroup(Pages[i - 1].GetComponent<CanvasGroup>(), 1));
                p = i;
            }
            AudioKit.PlaySound("023PaperOut");
        }
        private void Cover()
        {
            AudioKit.PlaySound("023PaperOut");
            foreach (var i in Pages)
            {
                i.GetComponent<CanvasGroup>().alpha = 0f;
                i.gameObject.SetActive(false);
                p = 0;
            }
        }
    }
}