using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DataSystem;
using QFramework;
using Kuchinashi;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Puzzle.Puzzle3
{
    public class Puzzle3 : PuzzleBase
    {
        public static Puzzle3 Instance;

        public static bool IsHoldingDoor = false;

        public List<Character> Characters = new List<Character>();
        public bool IsUnlocked => Numbers[Characters[0].data] == 7 && Numbers[Characters[1].data] == 7 && Numbers[Characters[2].data] == 7;
        public SerializableDictionary<CharacterData, int> Numbers;
        public Dictionary<int, CharacterData> NumbersReverse => Numbers.ToDictionary(x => x.Value, x => x.Key);

        private Button backButton;
        private Coroutine CurrentCoroutine = null;

        private void Awake()
        {
            Instance = this;
        }

        public void UpdateNumber(int pos, int direction)
        {
            var newValue = Numbers[Characters[pos].data] + direction;
            newValue = (newValue + 8) % 8;

            var newCharacter = NumbersReverse[newValue];
            UserDictionary.Unlock(newCharacter.Id);

            Characters[pos].Initialize(newCharacter, true, true);
            AudioKit.PlaySound("Cube-Slide");
        }

        public override void OnEnter()
        {
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

        public override void OnExit()
        {
            base.OnExit();

            if (CurrentCoroutine != null)
            {
                StopCoroutine(CurrentCoroutine);
                CurrentCoroutine = null;
            }
        }
    }
}