using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DataSystem
{
    public enum PuzzleType
    {
        Puzzle,
        Hint,
        Dialog
    }

    public abstract class PuzzleDataBase : ScriptableObject, IHaveId
    {
        [SerializeField] private string id;
        public string Id => id;

        public abstract PuzzleType Type { get; }
        public GameObject Prefab;
        public Sprite Thumbnail;
        public string Name;
        [Multiline] public string Description;
    }

    [CreateAssetMenu(fileName = "PuzzleData", menuName = "Scriptable Objects/Puzzle Data", order = 0)]
    public class PuzzleData : PuzzleDataBase
    {
        public override PuzzleType Type => PuzzleType.Puzzle;
    }

    public interface IHaveId
    {
        string Id { get; }
    }
}