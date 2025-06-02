using UnityEngine;
using TMPro;  // Required for TextMeshPro

public class PlayerMovement : MonoBehaviour
{
    public float thrustForce = 5f;
    public float maxSpeed = 10f;

    public GameObject background;
    public TMP_Text coordinateText;
    public TMP_Text velocityText;



    private Rigidbody2D rb;
    private Vector2 input;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 0f;               // No friction
        rb.gravityScale = 0f;       // No gravity
    }
    void Update()
    {
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        if (coordinateText != null)
        {
            Vector2 pos = transform.position;
            coordinateText.text = $"X: {pos.x:F2}\nY: {pos.y:F2}";
        }
        if (velocityText != null)
        {
            float speed = rb.linearVelocity.magnitude; // Units per second
            velocityText.text = $"Velocity: {speed:F2} m/s";
        }


    }
    void FixedUpdate()
    {
        rb.AddForce(input.normalized * thrustForce);

        // Clamp max speed
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // WrapPlayerAroundMap();

    }

    void WrapPlayerAroundMap()
    {
        if (background == null) return;

        SpriteRenderer sr = background.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        Bounds bounds = sr.bounds;
        Vector3 pos = transform.position;

        float left = bounds.min.x;
        float right = bounds.max.x;
        float bottom = bounds.min.y;
        float top = bounds.max.y;

        if (pos.x > right) pos.x = left;
        else if (pos.x < left) pos.x = right;

        if (pos.y > top) pos.y = bottom;
        else if (pos.y < bottom) pos.y = top;

        transform.position = pos;
    }


}