using QFramework;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectsDetector : MonoBehaviour
{
    public float radius;
    public LayerMask targetLayer;

    Collider2D[] colliders;
    public bool touchable => Physics2D.OverlapCircle(transform.position, radius, targetLayer);
    public InteractiveObject DetectClosestObject()
    {
        colliders = Physics2D.OverlapCircleAll(transform.position, radius, targetLayer);
        if (colliders.IsNullOrEmpty()) return null;
        float minDist = 2*radius + 1;
        InteractiveObject closestObject = null;
        foreach(Collider2D o in colliders)
        {
            float dist = (o.Position2D() - transform.Position2D()).magnitude;
            if (dist < minDist)
            {
                minDist = dist;
                closestObject = o.transform.GetComponent<InteractiveObject>();
            }
        }
        return closestObject;
    }
    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, radius);
    }
}