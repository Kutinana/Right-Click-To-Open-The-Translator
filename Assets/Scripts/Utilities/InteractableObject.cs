using System.Collections;
using System.Collections.Generic;
using Cameras;
using DataSystem;
using UnityEngine;
using UnityEngine.UI;

public class InteractableObject : MonoBehaviour
{
    public ObtainableObjectData Data;

    private Image image;
    public bool FollowActive = false;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    void Update()
    {
        if (FollowActive) FollowMouseMove();
        if (Input.GetMouseButtonUp(1) || Input.GetKeyUp(KeyCode.Escape)) Destroy(gameObject);
    }

    public InteractableObject Initialize(ObtainableObjectData data)
    {
        Data = data;

        image.sprite = data.Sprite;
        FollowActive = true;

        return this;
    }

    public void FollowMouseMove()
    {
        var rootCanvas = transform.root.GetComponentInChildren<Canvas>();

        if (rootCanvas.renderMode is RenderMode.ScreenSpaceCamera)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.GetComponent<RectTransform>(),
                Input.mousePosition, rootCanvas.worldCamera, out var pos);
            GetComponent<RectTransform>().anchoredPosition = pos;
        }
        else if (rootCanvas.renderMode is RenderMode.ScreenSpaceOverlay)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.GetComponent<RectTransform>(),
                Input.mousePosition, PuzzleCameraManager.Camera, out var pos);
            GetComponent<RectTransform>().anchoredPosition = pos;
        }
    }

    public void DropAndExit()
    {
        FollowActive = false;
        StartCoroutine(DropAndExitCoroutine());
    }

    private IEnumerator DropAndExitCoroutine()
    {
        while (!Mathf.Approximately(image.color.a, 0f))
        {
            transform.localPosition = new Vector2(transform.localPosition.x, transform.localPosition.y - Time.deltaTime * 100f);
            image.color = new Color(image.color.r, image.color.g, image.color.b, image.color.a - Time.deltaTime * 2f);
            yield return new WaitForEndOfFrame();
        }

        Destroy(gameObject);
    }
}