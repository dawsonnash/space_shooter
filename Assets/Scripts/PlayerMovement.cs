using UnityEngine;
using TMPro;  // Required for TextMeshPro

public class PlayerMovement : MonoBehaviour
{
    public float thrustForce = 5f;
    public float maxSpeed = 10f;
    public float boostMultiplier = 2f;
    public float boostMaxSpeedMultiplier = 2f;

    public GameObject background;
    public TMP_Text coordinateText;
    public TMP_Text velocityText;

    public GameObject thrusterLeft;
    public GameObject thrusterRight;
    public GameObject boosterLeft;
    public GameObject boosterRight;

    private FuelManager fuelManager;



    public KeyCode boostKey = KeyCode.LeftControl;
    public bool IsBoosting()
    {
        bool pressW = Input.GetKey(KeyCode.W);
        bool pressA = Input.GetKey(KeyCode.A);
        bool pressD = Input.GetKey(KeyCode.D);

        return Input.GetKey(boostKey) && (pressW || pressA || pressD);
    }

    private Rigidbody2D rb;
    private Vector2 input;

    private AudioSource thrustAudio;
    private float targetVolume = 0f;
    private float fadeSpeed = 2f; // How fast volume changes (higher = faster)

    private AudioSource boosterAudio;
    private float boosterTargetVolume = 0f;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.linearDamping = 0f;               // No friction
        rb.gravityScale = 0f;       // No gravity

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
        input.y = Input.GetAxis("Vertical"); // Only forward/back input (W/S)

        bool pressW = Input.GetKey(KeyCode.W);
        bool pressS = Input.GetKey(KeyCode.S);
        bool pressA = Input.GetKey(KeyCode.A);
        bool pressD = Input.GetKey(KeyCode.D);
        bool isThrusting = Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D);

        bool isMovementInput = pressW || pressA || pressD; // ignore S
        bool boosting = Input.GetKey(boostKey) &&
                        (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) &&
                        fuelManager != null && fuelManager.HasFuel();


        bool usingLeftThruster = (pressW || pressD);
        bool usingRightThruster = (pressW || pressA);


        boosterTargetVolume = boosting ? 1f : 0f;

        if (boosterAudio != null)
        {
            if (!boosterAudio.isPlaying && boosterTargetVolume > 0f)
                boosterAudio.Play();

            boosterAudio.volume = Mathf.MoveTowards(boosterAudio.volume, boosterTargetVolume, fadeSpeed * Time.deltaTime);

            if (boosterAudio.volume == 0f && boosterAudio.isPlaying)
                boosterAudio.Stop();
        }

        targetVolume = isMovementInput ? 1f : 0f;

        if (thrustAudio != null)
        {
            if (!thrustAudio.isPlaying && targetVolume > 0f)
                thrustAudio.Play();

            thrustAudio.volume = Mathf.MoveTowards(thrustAudio.volume, targetVolume, fadeSpeed * Time.deltaTime);

            if (thrustAudio.volume == 0f && thrustAudio.isPlaying)
                thrustAudio.Stop();
        }




        // Regular thrusters always follow input
        thrusterLeft.SetActive(usingLeftThruster);
        thrusterRight.SetActive(usingRightThruster);

        // Boosters appear *in addition* when boost key is held
        boosterLeft.SetActive(usingLeftThruster && boosting);
        boosterRight.SetActive(usingRightThruster && boosting);


        if (coordinateText != null)
        {
            Vector2 pos = transform.position;
            coordinateText.text = $"X: {pos.x:F2}\nY: {pos.y:F2}";
        }

        if (velocityText != null)
        {
            float speed = rb.linearVelocity.magnitude;
            velocityText.text = $"Velocity: {speed:F2} m/s";
        }
    }
    void FixedUpdate()
    {
        // Rotate the ship
        float rotationInput = 0f;
        if (Input.GetKey(KeyCode.A)) rotationInput = 1f;
        if (Input.GetKey(KeyCode.D)) rotationInput = -1f;

        float torqueAmount = 0.6f; // tweak to taste
        rb.AddTorque(rotationInput * torqueAmount);


        // Determine current thrust and max speed based on boost state
        bool boosting = IsBoosting() && fuelManager != null && fuelManager.HasFuel();
        float currentThrust = boosting ? thrustForce * boostMultiplier : thrustForce;
        float currentMaxSpeed = boosting ? maxSpeed * boostMaxSpeedMultiplier : maxSpeed;

        // Apply forward or reverse thrust
        if (input.y != 0)
        {
            Vector2 thrustDirection = transform.up;
            rb.AddForce(thrustDirection * input.y * currentThrust);
        }

        // Clamp velocity to max speed
        if (rb.linearVelocity.magnitude > currentMaxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * currentMaxSpeed;
        }


    }



}