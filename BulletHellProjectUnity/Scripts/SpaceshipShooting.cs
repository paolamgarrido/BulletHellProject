using UnityEngine;
using System.Collections;
using TMPro;

public class SpaceshipShooting : MonoBehaviour
{
    public GameObject bulletPrefab; // Reference to the bullet prefab for instantiation
    public Transform bulletSpawnPoint; // Spawn point for bullets on the spaceship
    public TextMeshProUGUI bulletCounterText; // UI element to display the number of bullets fired
    
    public float bulletSpeed = 20f; // Speed at which bullets travel
    public float fireRate = 0.4f; // Time interval between successive bullets
    public float bulletLifetime = 5f; // Lifetime of a bullet before it gets destroyed
    public float spawnOffset = 1.0f; // Distance from the spaceship to spawn bullets

    private int bulletsPerCircle = 6; // Number of bullets in circular firing pattern
    private float nextFireTime = 0f; // Time tracker for controlling fire rate
    private int bulletCount = 0; // Tracks the total number of bullets fired
    private float globalOffset = 0f; // Global angle offset for dynamic bullet patterns

    private float timer = 0f; // Tracks elapsed time for pattern duration
    private float patternDuration = 10f; // Active duration of a firing pattern
    private float pauseDuration = 2f; // Pause duration between patterns
    private bool isPaused = false; // Determines whether the system is in pause mode
    private int currentPattern = 1; // Tracks the active firing pattern (1, 2, or 3)

    // Validate assigned references and warn about missing ones
    void Start()
    {
        if (!bulletPrefab || !bulletSpawnPoint)
        {
            Debug.LogError("Critical references are missing! Ensure bulletPrefab and bulletSpawnPoint are assigned.");
        }
    }

    void Update()
    {
        // Update the timer for pattern control
        timer += Time.deltaTime;

        if (isPaused)
        {
            // Handle pause phase
            if (timer >= pauseDuration)
            {
                isPaused = false; 

                // Reset timer
                timer = 0f;

                // Cycle to the next pattern and loop to pattern 1
                currentPattern++; 
                if (currentPattern > 3) currentPattern = 1; 
            }
        }
        else
        {
            // Handle active phase of the pattern
            if (timer < patternDuration)
            {
                // Fire bullets if the time interval allows
                if (Time.time >= nextFireTime)
                {
                    switch (currentPattern)
                    {
                        case 1:
                            Pattern1();
                            break;
                        case 2:
                            Pattern2();
                            break;
                        case 3:
                            Pattern3();
                            break;
                    }

                    // Set the next allowed fire time
                    nextFireTime = Time.time + fireRate;
                }
            }
            else
            {
                // Transition to pause phase
                isPaused = true;
                timer = 0f; 
            }
        }
    }

    // Pattern 1: Double Spiral
    void Pattern1()
    {
        // Angle between each bullet and cycle
        float angleStep = 360f / bulletsPerCircle; 
        float angle = globalOffset; 

        for (int i = 0; i < bulletsPerCircle; i++)
        {
            SpawnBullet(angle); 
            angle += angleStep; 
            angle %= 360f; 
        }

        // Increment the global offset to create a dynamic rotation effect
        globalOffset += 4f;
        if (globalOffset > 16f) globalOffset = 0f;

        // Update the UI with the new bullet count
        UpdateBulletCounterUI(); 
    }

    // Pattern 2: Flower effect
    void Pattern2()
    {
        // Angle between each bullet and circle (clockwise & counterclockwise)
        float angleStep = 360f / bulletsPerCircle; 
        float clockwiseOffset = globalOffset; 
        float counterClockwiseOffset = -globalOffset; 

        for (int i = 0; i < bulletsPerCircle; i++)
        {
            // Spawn a clockwise bullet
            SpawnBullet(clockwiseOffset); 
            clockwiseOffset += angleStep; 
            clockwiseOffset %= 360f; 

            // Counterclockwise bullet with offset
            SpawnBullet(counterClockwiseOffset + (angleStep / 2)); 
            counterClockwiseOffset -= angleStep;
            if (counterClockwiseOffset < 0f) counterClockwiseOffset += 360f; 
        }

        // Increment global offset for a dynamic rotation effect
        globalOffset += 6f;
        globalOffset %= 360f;

        // Update UI
        UpdateBulletCounterUI(); 
    }

    // Pattern 3: Star-shaped spiral
    void Pattern3()
    {
        // Number of points in the star, angle separation and length
        int starPoints = 5; 
        float angleStep = 360f / starPoints; // Angular separation between points
        float length = spawnOffset + 5f; 

        for (int i = 0; i < starPoints; i++)
        {
            // Starting angle for the point
            float angle = globalOffset + i * angleStep; 

            // Spawn bullets along the point's line
            for (float dist = spawnOffset; dist <= length; dist += 1f)
            {
                Vector3 spawnPosition = bulletSpawnPoint.position + new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), 0, Mathf.Sin(angle * Mathf.Deg2Rad)) * dist;

                GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity); 

                Rigidbody rb = bullet.GetComponent<Rigidbody>();
                if (rb)
                {
                    rb.velocity = (spawnPosition - bulletSpawnPoint.position).normalized * bulletSpeed; 
                }
                // Increment the bullet count
                bulletCount++;
                
                // Destroy bullet after its lifetime
                Destroy(bullet, bulletLifetime); 
            }
        }

        // Rotate the star pattern
        globalOffset += 5f; 
        globalOffset %= 360f;

        // Update the UI
        UpdateBulletCounterUI(); 
    }

    // Spawn a single bullet at a specified angle
    void SpawnBullet(float angle)
    {
        float bulletDirX = Mathf.Cos(angle * Mathf.Deg2Rad);
        float bulletDirZ = Mathf.Sin(angle * Mathf.Deg2Rad);
        Vector3 bulletDirection = new Vector3(bulletDirX, 0, bulletDirZ).normalized;

        // Calculate spawn position
        Vector3 spawnPosition = bulletSpawnPoint.position + bulletDirection * spawnOffset; 

        // Instantiate the bullet
        GameObject bullet = Instantiate(bulletPrefab, spawnPosition, Quaternion.identity); 
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.velocity = bulletDirection * bulletSpeed; // Apply bullet speed and direction
        }

        bulletCount++; // Increment bullet count
        Destroy(bullet, bulletLifetime); // Destroy bullet after a defined lifetime
    }

    // Update the bullet count display in the UI
    void UpdateBulletCounterUI()
    {
        if (bulletCounterText)
        {
            bulletCounterText.text = "Bullets Fired: " + bulletCount; // Display updated bullet count
        }
    }
}