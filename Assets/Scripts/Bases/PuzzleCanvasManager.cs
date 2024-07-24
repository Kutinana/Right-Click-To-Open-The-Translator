using QFramework;

namespace Cameras
{
    public class PuzzleCanvasManager : MonoSingleton<PuzzleCanvasManager>
    {
        void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}