using UnityEngine;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    [Header("Thrust Settings")]
    public float normalAcceleration = 5f;
    public float boostAcceleration = 10f;
    public KeyCode boostKey = KeyCode.LeftControl;

    [Header("HUD")]
    public RectTransform movementIndicator;
    public float hudMaxOffset = 90f;
    public float hudSpeedReference = 10f;  // 10 m/s maps to full HUD ring
    public RectTransform targetIndicator;
    public float hudTargetRadius = 90f; // same as HUD ring size
    public Transform trackedTarget; // assign this when a planet is selected


    [Header("UI Text")]
    public TMP_Text coordinateText;
    public TMP_Text velocityText;

    [Header("Thruster Visuals")]
    public GameObject thrusterLeft;
    public GameObject thrusterRight;
    public GameObject boosterLeft;
    public GameObject boosterRight;

    private Rigidbody2D rb;
    private FuelManager fuelManager;
    private Vector2 input;

    private AudioSource thrustAudio;
    private AudioSource boosterAudio;
    private float fadeSpeed = 2f;
    private float targetVolume = 0f;
    private float boosterTargetVolume = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 0f;
        rb.gravityScale = 0f;

        fuelManager = FindFirstObjectByType<FuelManager>();

        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            thrustAudio = sources[0];
            boosterAudio = sources[1];
        }
    }

    void Update()
    {
        HandleInput();
        UpdateAudio();
        UpdateThrusterVisuals();
        UpdateUI();
        UpdateHUDIndicator();
        UpdateTargetIndicator();
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandleThrust();
    }

    // ------------------------ Logic Breakdown ------------------------

    void HandleInput()
    {
        input.y = Input.GetAxis("Vertical");
    }

    public bool IsBoosting()
    {
        bool pressW = Input.GetKey(KeyCode.W);
        bool pressA = Input.GetKey(KeyCode.A);
        bool pressD = Input.GetKey(KeyCode.D);

        return Input.GetKey(boostKey) && (pressW || pressA || pressD) &&
               fuelManager != null && fuelManager.HasFuel();
    }

    void HandleRotation()
    {
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.A)) rotationInput = 1f;
        if (Input.GetKey(KeyCode.D)) rotationInput = -1f;

        float torqueAmount = 0.6f;
        rb.AddTorque(rotationInput * torqueAmount);
    }

    void HandleThrust()
    {
        if (input.y > 0)
        {
            float accel = IsBoosting() ? boostAcceleration : normalAcceleration;
            Vector2 thrustDirection = transform.up;
            rb.AddForce(thrustDirection * input.y * accel);
        }
    }

    void UpdateAudio()
    {
        bool movementActive = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);
        targetVolume = movementActive ? 1f : 0f;
        boosterTargetVolume = IsBoosting() ? 1f : 0f;

        if (thrustAudio != null)
        {
            if (!thrustAudio.isPlaying && targetVolume > 0f) thrustAudio.Play();
            thrustAudio.volume = Mathf.MoveTowards(thrustAudio.volume, targetVolume, fadeSpeed * Time.deltaTime);
            if (thrustAudio.volume == 0f && thrustAudio.isPlaying) thrustAudio.Stop();
        }

        if (boosterAudio != null)
        {
            if (!boosterAudio.isPlaying && boosterTargetVolume > 0f) boosterAudio.Play();
            boosterAudio.volume = Mathf.MoveTowards(boosterAudio.volume, boosterTargetVolume, fadeSpeed * Time.deltaTime);
            if (boosterAudio.volume == 0f && boosterAudio.isPlaying) boosterAudio.Stop();
        }
    }

    void UpdateThrusterVisuals()
    {
        bool pressW = Input.GetKey(KeyCode.W);
        bool pressA = Input.GetKey(KeyCode.A);
        bool pressD = Input.GetKey(KeyCode.D);

        bool usingLeftThruster = pressW || pressD;
        bool usingRightThruster = pressW || pressA;
        bool boosting = IsBoosting();

        thrusterLeft.SetActive(usingLeftThruster);
        thrusterRight.SetActive(usingRightThruster);
        boosterLeft.SetActive(usingLeftThruster && boosting);
        boosterRight.SetActive(usingRightThruster && boosting);
    }

    void UpdateUI()
    {
        if (coordinateText != null)
        {
            Vector2 pos = transform.position;
            coordinateText.text = $"X: {pos.x:F2}\nY: {pos.y:F2}";
        }

        if (velocityText != null)
        {
            Vector2 velocity = rb.linearVelocity;
            float speed = velocity.magnitude;

            // Determine angle between velocity and ship's facing
            float angle = Vector2.Angle(velocity, transform.up);

            // Use sign based on angle (if angle > 90°, you're moving backward)
            float signedSpeed = (angle > 90f) ? -speed : speed;

            velocityText.text = $"{signedSpeed:F2} m/s";


        }
    }

    void UpdateHUDIndicator()
    {
        if (movementIndicator == null || rb == null) return;

        Vector2 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        Vector2 hudDirection = localVelocity / hudSpeedReference;

        if (hudDirection.magnitude > 1f)
            hudDirection = hudDirection.normalized;

        Vector2 offset = hudDirection * hudMaxOffset;
        movementIndicator.anchoredPosition = offset;
    }

    void UpdateTargetIndicator()
    {
        if (targetIndicator == null) return;

        if (trackedTarget == null)
        {
            targetIndicator.gameObject.SetActive(false);
            return;
        }

        targetIndicator.gameObject.SetActive(true);

        Vector2 toTarget = (trackedTarget.position - transform.position).normalized;

        float angleToTarget = Mathf.Atan2(toTarget.y, toTarget.x) * Mathf.Rad2Deg;
        float playerAngle = transform.eulerAngles.z;
        float relativeAngle = angleToTarget - playerAngle;
        float angleRad = relativeAngle * Mathf.Deg2Rad;

        Vector2 hudPos = new Vector2(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * hudTargetRadius;
        targetIndicator.anchoredPosition = hudPos;

        targetIndicator.rotation = Quaternion.Euler(0, 0, angleToTarget);
    }

}
