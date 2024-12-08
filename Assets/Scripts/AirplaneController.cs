using System;
using System.Runtime.ConstrainedExecution;
using UnityEngine;
using UnityEngine.UI;

public class AirplaneController : MonoBehaviour
{
    // UI 
    public Slider fuelBar;

    // Game Objects
    public GameObject propeller, plane, balloons, rudder, ruling, eleronLeft, eleronRight;

    // Audio Sources
    public AudioSource propellerSound, crashSound, shootSound, wind;


    // Game Management
    public GameManager GameManager;
    public float startProtectionTime = 5f;

    // Fuel Settings
    public float fuel = 100f;
    private const float FUEL_CONS_RATE = 0.02f, MAX_FUEL = 100.0f, FUEL_REFILL_RATE = 0.1f;


    // Speed and Movement Settings
    public float speed = 0.0f, maxSpeed = 100.0f, minSpeed = 0.0f, acceleration = 10.0f, rotationSpeed = 100.0f, angularSpeed = 500f;

    // Rigidbody
    private Rigidbody rb;

    // Shooting Settings
    public GameObject rocketPrefab;
    public Transform bulletSpawnPoint;
    public float rocketSpeed = 50f, fireRate = 0.5f, shootingRange = 100f;

    // Camera
    public Camera playerCamera;
    public Transform PlayerCamera;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        HandleSpeedControl();
        updateFuelBar();
        //HandleFlaps();
        handleRudder();
        handleAilerons();
        handleRuling();
    }

   private void handleRuling()
{
    float targetRulingRotation;

    // Check for W and S inputs to control ruling movement
    if (Input.GetKey(KeyCode.W))
    {
        // When W is pressed, set the ruling to pitch down
        targetRulingRotation = -25f; // Target rotation for W
    }
    else if (Input.GetKey(KeyCode.S))
    {
        // When S is pressed, set the ruling to pitch up
        targetRulingRotation = 40f; // Target rotation for S
    }
    else
    {
        // No input, reset ruling to the neutral position (0 degrees)
        targetRulingRotation = 0f;
    }

    // Get the current X rotation of the ruling to apply smooth interpolation
    float currentRulingRotation = ruling.transform.localEulerAngles.x;
    if (currentRulingRotation > 180) currentRulingRotation -= 360; // Ensure it's within -180 to 180 range

    // Smoothly interpolate the X rotation towards the target rotation
    float newRulingRotation = Mathf.LerpAngle(currentRulingRotation, targetRulingRotation, Time.deltaTime * 5); // Adjust speed as needed

    // Apply the calculated rotation to the ruling, preserving Y and Z rotations
    ruling.transform.localRotation = Quaternion.Euler(newRulingRotation, ruling.transform.localEulerAngles.y, ruling.transform.localEulerAngles.z);
}



    private void handleAilerons()
    {
        float targetLeftAileronRotation;
        float targetRightAileronRotation;

        // Check for Q and E inputs to control aileron movement
        if (Input.GetKey(KeyCode.Q))
        {
            // When Q is pressed, left aileron goes up and right aileron goes down
            targetLeftAileronRotation = 50f;    // Left aileron up
            targetRightAileronRotation = -50f;  // Right aileron down
        }
        else if (Input.GetKey(KeyCode.E))
        {
            // When E is pressed, right aileron goes up and left aileron goes down
            targetLeftAileronRotation = -50f;   // Left aileron down
            targetRightAileronRotation = 50f;   // Right aileron up
        }
        else
        {
            // No input, reset both ailerons to the neutral position (0 degrees)
            targetLeftAileronRotation = 4.989f;
            targetRightAileronRotation = 4.989f;
        }

        // Smoothly transition to the target rotation to avoid abrupt snapping
        Vector3 leftAileronRotation = eleronLeft.transform.localEulerAngles;
        Vector3 rightAileronRotation = eleronRight.transform.localEulerAngles;

        eleronLeft.transform.localRotation = Quaternion.Euler(
            Mathf.LerpAngle(leftAileronRotation.x, targetLeftAileronRotation, Time.deltaTime * 5), // Adjust 5 to control speed
            leftAileronRotation.y,
            leftAileronRotation.z
        );

        eleronRight.transform.localRotation = Quaternion.Euler(
            Mathf.LerpAngle(rightAileronRotation.x, targetRightAileronRotation, Time.deltaTime * 5), // Adjust 5 to control speed
            rightAileronRotation.y,
            rightAileronRotation.z
        );
    }

private void handleRudder()
{
    float targetRudderRotation;

    // Check for A and D inputs to control rudder movement
    if (Input.GetKey(KeyCode.A))
    {
        // When A is pressed, rudder goes left
        targetRudderRotation = 40f; // Rudder left
    }
    else if (Input.GetKey(KeyCode.D))
    {
        // When D is pressed, rudder goes right
        targetRudderRotation = -40f; // Rudder right
    }
    else
    {
        // No input, reset rudder to the neutral position (0 degrees)
        targetRudderRotation = 0f;
    }

    // Get the current Y rotation of the rudder, ensuring it’s within -180 to 180 for correct handling
    float currentRudderRotation = rudder.transform.localEulerAngles.y;
    if (currentRudderRotation > 180) currentRudderRotation -= 360;

    // Smoothly interpolate the Y rotation towards the target rotation
    float newRudderRotation = Mathf.Lerp(currentRudderRotation, targetRudderRotation, Time.deltaTime * 5); // Adjust 5 to control speed

    // Apply the calculated Y rotation to the rudder, keeping X and Z rotations the same
    rudder.transform.localRotation = Quaternion.Euler(
        rudder.transform.localEulerAngles.x, // Preserve X rotation
        newRudderRotation,                   // Set Y rotation
        rudder.transform.localEulerAngles.z  // Preserve Z rotation
    );
}



    private void HandleSpeedControl()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed += acceleration * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            speed -= acceleration * Time.deltaTime;
        }

        speed = Mathf.Clamp(speed, minSpeed, maxSpeed);
    }
    //     private void HandleFlaps()
    // {
    //     // Calculate pitch and roll based on the plane's rotation
    //     float pitch = transform.eulerAngles.x;
    //     float roll = transform.eulerAngles.z;

    // Debug.Log("Pitch: " + pitch + " Roll: " + roll);
    //     // Adjust pitch to be within -180 to 180 range for better handling of rotations
    //     if (pitch > 180) pitch -= 360;
    //     if (roll > 180) roll -= 360;

    //     // Map pitch to flap rotation angles
    //     float targetFlapRotation = Mathf.Lerp(5.18f, -40f, Mathf.InverseLerp(-30, 30, pitch));

    //     // Apply the target rotation to both flaps along the X axis
    //     rightFlap.transform.localRotation = Quaternion.Euler(targetFlapRotation, rightFlap.transform.localRotation.y, rightFlap.transform.localRotation.z);
    //     leftFlap.transform.localRotation = Quaternion.Euler(targetFlapRotation, leftFlap.transform.localRotation.y, leftFlap.transform.localRotation.z);

    //     // Optional: You can add roll-based adjustments for even more realistic flap movements
    //     float rollAdjustment = Mathf.Lerp(-10f, 10f, Mathf.InverseLerp(-30, 30, roll));
    //     rightFlap.transform.localRotation *= Quaternion.Euler(0, rollAdjustment, 0);
    //     leftFlap.transform.localRotation *= Quaternion.Euler(0, -rollAdjustment, 0);
    // }


    private void HandleMovement()
    {
        //MOUSE MOVEMENT
        float rotationAboutY;
        float rotationAboutX;

        rotationAboutX = -Input.GetAxis("Mouse Y") * angularSpeed * Time.deltaTime;
        transform.Rotate(rotationAboutX, 0, 0);

        rotationAboutY = Input.GetAxis("Mouse X") * angularSpeed * Time.deltaTime;
        transform.Rotate(0, rotationAboutY, 0);

        Animator animator = propeller.GetComponent<Animator>();

        //KEYBOARD MOVEMENT

        float pitch = Input.GetAxis("Vertical") * rotationSpeed * Time.deltaTime;
        transform.Rotate(pitch, 0, 0);


        float yaw = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, yaw, 0);


        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);//roll
        }

        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(0, 0, -rotationSpeed * Time.deltaTime);
        }


        //FORWARD 
        if (speed > 0)
        {
            animator.SetBool("isMoving", true);
            Vector3 move = transform.forward * speed * Time.deltaTime;
            rb.MovePosition(rb.position + move);
            if (!propellerSound.isPlaying)
            {
                propellerSound.Play();
            }
        }
        else
        {
            animator.SetBool("isMoving", false);
            propellerSound.Stop();
        }
    }


    private void HandleShooting()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Mouse0))
        {
            shootSound.Play();
            GameObject rocket = Instantiate(rocketPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);//new rocket instance in spawn point
            Rigidbody rocketRb = rocket.GetComponent<Rigidbody>();
            rocketRb.velocity = bulletSpawnPoint.forward * rocketSpeed;
            Destroy(rocket, 5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (GameManager.elapsedTime < startProtectionTime) return;

        if (collision.gameObject.CompareTag("Terrain") || collision.gameObject.CompareTag("Air baloon"))
        {
            crashSound.Play();
            Debug.Log("Plane hit the terrain! Game Over.");
            GameOver();
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Fuel"))
        {
            fuel += FUEL_REFILL_RATE;
            fuel = Mathf.Clamp(fuel, 0, 100);
            fuelBar.value = fuel / (float)MAX_FUEL;
            AudioSource parentAudioSource = other.transform.parent.GetComponent<AudioSource>();
            if (parentAudioSource != null)
            {
                parentAudioSource.PlayOneShot(parentAudioSource.clip);
            }
            Destroy(other.gameObject);
        }
    }
    void updateFuelBar()
    {
        if (propeller.GetComponent<Animator>().GetBool("isMoving"))
        {
            fuel -= FUEL_CONS_RATE;
            fuel = Mathf.Clamp(fuel, 0, 100);
            fuelBar.value = fuel / (float)MAX_FUEL;
        }
        if (fuel <= 0)
        {
             GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        enabled = false;
        if (wind.isPlaying)
        {
            wind.Stop();
        }
        if (propellerSound.isPlaying)
        {
            propellerSound.Stop();
        }
        GameManager.EndGame();
    }

}
