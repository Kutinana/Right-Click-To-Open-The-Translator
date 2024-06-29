using QFramework;
using System.Collections;
using UnityEngine;

public class MapController: MonoSingleton<MapController>
{
    public enum MapState
    {
        Closing,
        Opening
    }

    private MapState m_mapState;
    public MapState currentMapState
    {
        get { return m_mapState; }
    }
    CanvasGroup canvasGroup;
    
    private void Awake()
    {
        canvasGroup = GameObject.Find("MapCanvas").GetComponent<CanvasGroup>();
    }

    public static void OpenMap()
    {
        Instance.StartCoroutine(Instance.MapOpening());
        Instance.m_mapState = MapState.Opening;
    }

    public static void CloseMap()
    {
        Instance.StartCoroutine(Instance.MapClosing());
        Instance.m_mapState = MapState.Closing;
    }

    public static void MoveMap()
    {

    }

    public static void NextLevel()
    {

    }

    public static void PreviousLevel()
    {

    }

    private IEnumerator MapOpening()
    {
        yield return MapFadingOrAppearing(canvasGroup, 1f);

        
    }

    private IEnumerator MapClosing()
    {
        yield return MapFadingOrAppearing(canvasGroup, 0f);


    }

    private IEnumerator MapFadingOrAppearing(CanvasGroup canvasGroup, float alpha, float speed = 0.05f, float delay = 0f)
    {
        if (canvasGroup.alpha == alpha) yield break;
        yield return new WaitForSeconds(delay);
        
        while (!Mathf.Approximately(canvasGroup.alpha, alpha))
        {
            canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, alpha, speed);
            yield return new WaitForFixedUpdate();
        }
        canvasGroup.alpha = alpha;
    }
}