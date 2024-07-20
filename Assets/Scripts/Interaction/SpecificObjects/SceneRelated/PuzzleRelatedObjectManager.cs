using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataSystem;

public class PuzzleRelatedObjectManager: MonoBehaviour
{
    public List<string> ObjectRelatedPuzzles;
    public List<Transform> PuzzleRelatedObjects;
    public List<string> PuzzleRelatedPuzzles;
    public List<Transform> Puzzles;
    public List<string> DoorRelatedPuzzles;
    public List<Transform> RelatedDoors;
    private void Awake()
    {
        DisableObjects();
        DisablePuzzles();
    }

    private void DisableObjects()
    {
        for (int i = 0; i < ObjectRelatedPuzzles.Count; i++)
        {
            if (GameProgressData.GetPuzzleProgress(ObjectRelatedPuzzles[i]) == PuzzleProgress.Solved) PuzzleRelatedObjects[i].gameObject.SetActive(false);
        }
    }

    private void DisablePuzzles()
    {
        for (int i = 0; i < PuzzleRelatedPuzzles.Count; i++)
        {
            if (GameProgressData.GetPuzzleProgress(PuzzleRelatedPuzzles[i]) == PuzzleProgress.UnSolved
                || GameProgressData.GetPuzzleProgress(PuzzleRelatedPuzzles[i]) == PuzzleProgress.NotFound) Puzzles[i].GetComponent<InteractivePuzzle>().SetActivable(false);
            else if (GameProgressData.GetPuzzleProgress(PuzzleRelatedPuzzles[i]) == PuzzleProgress.Solved) Puzzles[i].GetComponent<InteractivePuzzle>().SetActivable(true);
        }
    }
    private void DisableDoors()
    {
        for (int i = 0; i < PuzzleRelatedPuzzles.Count; i++)
        {
            if (GameProgressData.GetPuzzleProgress(PuzzleRelatedPuzzles[i]) == PuzzleProgress.UnSolved
                || GameProgressData.GetPuzzleProgress(PuzzleRelatedPuzzles[i]) == PuzzleProgress.NotFound) Puzzles[i].GetComponent<InteractiveDoor>().SetActivable(false);
            else if (GameProgressData.GetPuzzleProgress(PuzzleRelatedPuzzles[i]) == PuzzleProgress.Solved) Puzzles[i].GetComponent<InteractiveDoor>().SetActivable(true);
        }
    }
}