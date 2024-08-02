using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DataSystem;
using Unity.VisualScripting;

public class PuzzleRelatedObjectManager: MonoBehaviour
{
    public List<string> ObjectRelatedPuzzles;
    public List<Transform> PuzzleRelatedDisableObjects;
    public List<Transform> PuzzleRelatedEnableObjects;
    public List<string> PuzzleRelatedPuzzles;
    public List<Transform> Puzzles;
    public List<string> objectID;
    public List<Transform> CorrespondingObjects;

    private void Awake()
    {

    }

    private void Start()
    {
        DisableObjects();
        DisablePuzzles();
        ObjectPicked();
    }

    private void DisableObjects()
    {
        for (int i = 0; i < ObjectRelatedPuzzles.Count; i++)
        {
            if (GameProgressData.GetPuzzleProgress(ObjectRelatedPuzzles[i]) == PuzzleProgress.Solved) PuzzleRelatedDisableObjects[i].gameObject.SetActive(false);
        }
    }
    private void EnableObjects()
    {
        for (int i = 0; i < ObjectRelatedPuzzles.Count; i++)
        {
            if (GameProgressData.GetPuzzleProgress(ObjectRelatedPuzzles[i]) == PuzzleProgress.Solved) PuzzleRelatedEnableObjects[i].gameObject.SetActive(false);
        }
    }

    private void DisablePuzzles()
    {
        for (int i = 0; i < PuzzleRelatedPuzzles.Count; i++)
        {
            if (GameProgressData.GetPuzzleProgress(PuzzleRelatedPuzzles[i]) == PuzzleProgress.UnSolved
                || GameProgressData.GetPuzzleProgress(PuzzleRelatedPuzzles[i]) == PuzzleProgress.NotFound) Puzzles[i].GetComponent<InteractiveObject>().SetActivable(false);
            else if (GameProgressData.GetPuzzleProgress(PuzzleRelatedPuzzles[i]) == PuzzleProgress.Solved) Puzzles[i].GetComponent<InteractiveObject>().SetActivable(true);
        }
    }
    private void ObjectPicked()
    {
        for (int i = 0; i < objectID.Count; i++)
        {
            if (GameProgressData.GetInventory().TryGetValue(objectID[i], out var value) && value >= 1)
            {
                Debug.Log(true);
                CorrespondingObjects[i].gameObject.SetActive(false);
            }
            else CorrespondingObjects[i].gameObject.SetActive(true);
        }
    }
}