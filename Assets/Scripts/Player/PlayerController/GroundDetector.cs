using Unity.VisualScripting;
using UnityEngine;

public class GroundDetector: MonoBehaviour
{
    public Vector2 pointA;
    public Vector2 pointB;
    public LayerMask layer;

    Collider2D[] colliders = new Collider2D[1];
    public bool isGrounded => Physics2D.OverlapArea(pointA, pointB, layer);
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pointA, new Vector2(pointB.x, pointA.y));
        Gizmos.DrawLine(new Vector2(pointA.x, pointB.y), pointB);
        Gizmos.DrawLine(pointA, new Vector2(pointA.x, pointB.y));
        Gizmos.DrawLine(pointB, new Vector2(pointB.x, pointA.y));
    }
}