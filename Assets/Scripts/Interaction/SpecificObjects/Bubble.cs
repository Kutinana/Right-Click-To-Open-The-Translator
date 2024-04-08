
using QFramework;
using UnityEngine;

public class Bubble: MonoBehaviour
{
    public float MaxSpeed = 2;
    public float height = 1.1f;
    public float DisappearSpeed = 2;

    bool up = false;
    bool disappear = false;

    private float parameter;
    private float upPara = 0;
    private Vector2 targetposition;
    private Vector2 currPosition;
    private Vector2 nextPosition;
    private SpriteRenderer spriteRenderer;
    private Color currColor;
    private float Alpha;
    private float alphaPara = 0;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        targetposition = new Vector2(transform.position.x, transform.position.y);
        currColor = spriteRenderer.color;
        Alpha = currColor.a;
    }
    public void Next()
    {
        targetposition = new Vector2(transform.position.x, targetposition.y + height);
        currPosition = new Vector2(transform.position.x, transform.position.y);
        upPara = 0;
        up = true;
    }
    public void StartDisappear() => disappear = true;
    private void FixedUpdate()
    {
        if (up)
        {
            HeightUpdate();
            transform.position = new Vector3(nextPosition.x, nextPosition.y, transform.position.z);
        }
        if (disappear)
        {
            Disappear();
            spriteRenderer.color = new Color(currColor.r, currColor.g, currColor.b, Alpha);
            Debug.Log(spriteRenderer.color);
            if (Alpha == 0)
            {
                Destroy(this.gameObject);
            }
        }
    }
    private void HeightUpdate()
    {
        parameter = Mathf.Abs(targetposition.y - transform.position.y) / height;
        upPara += MaxSpeed * parameter * Time.deltaTime;
        nextPosition = Vector2.Lerp(currPosition, targetposition, upPara);
        if (parameter < 0.01) up = false;
    }
    private void Disappear()
    {
        alphaPara += DisappearSpeed * Time.deltaTime;
        Alpha = Mathf.Lerp(currColor.a, 0, alphaPara);
    }
}