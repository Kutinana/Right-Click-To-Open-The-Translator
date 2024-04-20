namespace Hint
{
    public struct OnHintInitializedEvent
    {
        public HintBase hint;
        public OnHintInitializedEvent(HintBase _hint)
        {
            hint = _hint;
        }
    }
    
    public struct OnHintExitEvent
    {
        public HintBase hint;
        public OnHintExitEvent(HintBase _hint)
        {
            hint = _hint;
        }
    }
}