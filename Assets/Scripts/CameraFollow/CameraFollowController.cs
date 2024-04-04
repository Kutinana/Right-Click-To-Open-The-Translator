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
    private float PlayerSpeed => mrigidbody.velocity.x;
    private void Awake()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        mrigidbody = Player.GetComponent<Rigidbody2D>();
        playerController = Player.GetComponent<PlayerController>();
    }
    private void FixedUpdate()
    {
        if (Player != null)
        {
            PositionUpdate();
            transform.position = new Vector3(currentPosition.x, currentPosition.y, transform.position.z);
        }
    }
    private void PositionUpdate()
    {
        MaxCameraSpeed = playerController.GetCurrentMaxSpeed();
        TargetPosition.x = Mathf.Clamp(Player.transform.position.x, minX, maxX);
        TargetPosition.y = transform.position.y;
        parameter = Mathf.Abs(Player.transform.position.x - transform.position.x) / slowRange;
        currentPosition = Vector2.Lerp(transform.position, TargetPosition, MaxCameraSpeed * (parameter/(1+parameter)) * Time.deltaTime);
    }
}
