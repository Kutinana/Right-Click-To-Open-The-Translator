using QFramework;
using System.Collections;
using System.Runtime.Remoting.Messaging;

public class SceneInitialization : ISingleton
{
    public void OnSingletonInit(){ }

    private static SceneInitialization _instance;

    public static SceneInitialization Instance
    {
        private set => _instance = value;
        get
        {
            if (_instance == null)
            {
                _instance = new SceneInitialization();
            }
            return _instance;
        }
    }

    private IEnumerator CheckGameProgressData()
    {
        SceneControl.SceneControl.CanTransition = true;
        yield return null;
    }
}