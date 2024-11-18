using UnityEngine;

public class SpaceshipFloat : MonoBehaviour
{
    public float floatHeight = 2.0f; // Desired height above the ground
    public float floatForce = 10.0f; // Strength of the floating force
    public float damping = 5.0f; // Damping to stabilize floating
    public float bobbingSpeed = 2.0f; // Speed of the up-and-down motion
    public float bobbingAmount = 0.2f; // Height variation for the bobbing motion

    private Rigidbody rb;
    private float initialYPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Store the initial Y position for reference
        initialYPosition = transform.position.y; 
    }

    void FixedUpdate()
    {
        // Ensure floatHeight is not zero to avoid division by zero
        if (floatHeight <= 0)
        {
            Debug.LogWarning("floatHeight must be greater than 0!");
            return;
        }

        // Check distance to the ground
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;

        // Calculate the bobbing offset
        float bobbingOffset = Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;

        if (Physics.Raycast(ray, out hit, floatHeight + bobbingAmount))
        {
            // Ensure the hit distance is valid
            if (hit.distance > floatHeight + bobbingAmount)
            {
                Debug.LogWarning("Raycast hit distance exceeds valid range!");
                return;
            }

            // Calculate proportional height
            float proportionalHeight = Mathf.Clamp(((floatHeight + bobbingOffset) - hit.distance) / floatHeight, 0f, 1f);
            Vector3 appliedForce = Vector3.up * proportionalHeight * floatForce;

            // Apply the floating force
            rb.AddForce(appliedForce, ForceMode.Acceleration);

            // Apply damping for stability
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y * (1.0f - damping * Time.fixedDeltaTime), rb.linearVelocity.z);
        }
        else
        {
            Debug.LogWarning("Raycast did not hit any surface. Ensure the terrain is below the spaceship.");
        }
    }
}
