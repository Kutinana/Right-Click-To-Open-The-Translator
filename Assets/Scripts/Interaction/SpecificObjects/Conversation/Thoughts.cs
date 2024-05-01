using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class Thoughts: MonoBehaviour
{
    private Toggle[] toggles;
    public void Exit()
    {
        GameObject.Find("TempPlayer").GetComponent<PlayerInput>().EnableInputActions();
        Destroy(gameObject);
    }
}