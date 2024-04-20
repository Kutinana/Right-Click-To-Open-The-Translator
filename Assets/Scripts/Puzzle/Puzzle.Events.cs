namespace Puzzle
{
    public struct OnPuzzleInitializedEvent
    {
        public PuzzleBase puzzle;
        public OnPuzzleInitializedEvent(PuzzleBase _puzzle)
        {
            puzzle = _puzzle;
        }
    }

    public struct OnPuzzleExitEvent
    {
        public PuzzleBase puzzle;
        public OnPuzzleExitEvent(PuzzleBase _puzzle)
        {
            puzzle = _puzzle;
        }
    }

    public struct OnPuzzleSolvedEvent
    {
        public PuzzleBase puzzle;
        public OnPuzzleSolvedEvent(PuzzleBase _puzzle)
        {
            puzzle = _puzzle;
        }
    }
}