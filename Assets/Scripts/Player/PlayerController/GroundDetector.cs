using Unity.VisualScripting;
using UnityEngine;

public class GroundDetector: MonoBehaviour
{
    public float offset;
    public float radius;
    public LayerMask layer;

    Collider2D[] colliders = new Collider2D[1];
    public bool isGrounded => Physics2D.OverlapCircle(new Vector2(transform.position.x, transform.position.y - offset), radius, layer);
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(new Vector2(transform.position.x, transform.position.y - offset), radius);
        //Gizmos.DrawLine(pointA, new Vector2(pointB.x, pointA.y));
        //Gizmos.DrawLine(new Vector2(pointA.x, pointB.y), pointB);
        //Gizmos.DrawLine(pointA, new Vector2(pointA.x, pointB.y));
        //Gizmos.DrawLine(pointB, new Vector2(pointB.x, pointA.y));
    }
}