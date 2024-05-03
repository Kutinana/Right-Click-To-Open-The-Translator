using QFramework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class CameraFollowController : MonoBehaviour
{
    private GameObject Player;
    private Rigidbody2D mrigidbody;
    private PlayerController playerController;
    public float minX;
    public float maxX;
    public float slowRange;
    
    private float MaxCameraSpeed = 5;
    private float parameter;
    private Vector2 TargetPosition;
    private Vector2 currentPosition;

    private float defaultFieldofView;
    private float targetFieldofView => defaultFieldofView/1.5f;
    private float currentFieldofView;
    private float FoVParameter;
    public int zoomSignal = 0;
    private float PlayerSpeed => mrigidbody.velocity.x;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        mrigidbody = Player.GetComponent<Rigidbody2D>();
        playerController = Player.GetComponent<PlayerController>();
        defaultFieldofView = this.gameObject.GetComponent<Camera>().fieldOfView;
        currentFieldofView = defaultFieldofView;
    }
    private void FixedUpdate()
    {
        if (Player != null)
        {
            PositionUpdate();
            transform.position = new Vector3(currentPosition.x, currentPosition.y, transform.position.z);
            Zoom();
        }
    }
    private void PositionUpdate()
    {
        MaxCameraSpeed = playerController.GetCurrentMaxSpeed();
        if (transform.position.x < minX || transform.position.x > maxX)
        {
            TargetPosition.x = transform.position.x < minX ? minX + 0.5f : maxX - 0.5f;
            TargetPosition.y = transform.position.y;
            parameter = 1;
            currentPosition = Vector2.Lerp(transform.position, TargetPosition, MaxCameraSpeed * parameter * Time.deltaTime);
        }
        else
        {
            TargetPosition.x = Mathf.Clamp(Player.transform.position.x, minX, maxX);
            TargetPosition.y = transform.position.y;
            parameter = Mathf.Abs(Player.transform.position.x - transform.position.x) / slowRange;
            currentPosition = Vector2.Lerp(transform.position, TargetPosition, MaxCameraSpeed * (parameter/(1+parameter)) * Time.deltaTime);
        }
        
    }
    private void ZoomIn()
    {
        currentFieldofView = this.gameObject.GetComponent<Camera>().fieldOfView;
        FoVParameter = Mathf.Abs((targetFieldofView - currentFieldofView) / (defaultFieldofView - targetFieldofView));
        currentFieldofView = Mathf.Lerp(currentFieldofView, targetFieldofView, 8 * (FoVParameter / (1 + FoVParameter)) * Time.deltaTime);
    }
    private void ZoomOut()
    {
        currentFieldofView = this.gameObject.GetComponent<Camera>().fieldOfView;
        parameter = Mathf.Abs((defaultFieldofView - currentFieldofView) / (defaultFieldofView - targetFieldofView));
        currentFieldofView = Mathf.Lerp(currentFieldofView, defaultFieldofView, 50 * (FoVParameter / (1 + FoVParameter)) * Time.deltaTime);
    }
    private void Zoom()
    {
        switch (zoomSignal)
        {
            case 1:
                ZoomIn();
                if (currentFieldofView - targetFieldofView <= 0.5)
                {
                    currentFieldofView = targetFieldofView;
                    zoomSignal = 0;
                }
                break;
            case 2:
                ZoomOut();
                if (defaultFieldofView - currentFieldofView<= 0.5)
                {
                    currentFieldofView = defaultFieldofView;
                    zoomSignal = 0;
                }
                break;
            default:
                break;
        }
        this.gameObject.GetComponent<Camera>().fieldOfView = currentFieldofView;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(minX + 10.2f, 5.7f), new Vector2(minX - 10.2f, 5.7f));
        Gizmos.DrawLine(new Vector2(minX + 10.2f, 5.7f), new Vector2(minX + 10.2f, -5.7f));
        Gizmos.DrawLine(new Vector2(minX + 10.2f, -5.7f), new Vector2(minX - 10.2f, -5.7f));
        Gizmos.DrawLine(new Vector2(minX - 10.2f, -5.7f), new Vector2(minX - 10.2f, 5.7f));

        Gizmos.DrawLine(new Vector2(maxX + 10.2f, 5.7f), new Vector2(maxX - 10.2f, 5.7f));
        Gizmos.DrawLine(new Vector2(maxX + 10.2f, 5.7f), new Vector2(maxX + 10.2f, -5.7f));
        Gizmos.DrawLine(new Vector2(maxX + 10.2f, -5.7f), new Vector2(maxX - 10.2f, -5.7f));
        Gizmos.DrawLine(new Vector2(maxX - 10.2f, -5.7f), new Vector2(maxX - 10.2f, 5.7f));
    }
}
