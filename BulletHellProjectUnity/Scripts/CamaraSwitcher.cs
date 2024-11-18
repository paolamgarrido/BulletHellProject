using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    // Class to define the spaceship and the order in which its camera will be activated
    [System.Serializable]
    public class CameraOrder
    {
        public GameObject spaceship;  // Reference to the spaceship containing the camera
        public int order;             // Activation order of the camera
    }

    public CameraOrder[] cameraOrders; // Array of spaceships and their activation order
    public float switchInterval = 5.0f; // Time interval between each camera switch

    private Camera[] orderedCameras;   // Array of cameras sorted by their activation order
    private int currentCameraIndex = 0; // Index of the currently active camera
    private float timer = 0.0f;        // Timer to track time for switching cameras

    void Start()
    {
        // Initialize and sort the cameras based on "order" field
        orderedCameras = new Camera[cameraOrders.Length];
        for (int i = 0; i < cameraOrders.Length; i++)
        {
            // Retrieve the camera component from the spaceship
            Camera spaceshipCamera = cameraOrders[i].spaceship.GetComponentInChildren<Camera>();
            if (spaceshipCamera != null)
            {
                // Assign the camera to its position in the ordered array
                orderedCameras[cameraOrders[i].order] = spaceshipCamera;
            }
            else
            {
                Debug.LogWarning($"No camera found in the spaceship: {cameraOrders[i].spaceship.name}");
            }
        }

        // Ensure only the first camera is active at the start
        for (int i = 0; i < orderedCameras.Length; i++)
        {
            if (orderedCameras[i] != null)
            {
                // Activate the first camera and deactivate the others
                orderedCameras[i].gameObject.SetActive(i == currentCameraIndex);
            }
        }
    }

    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Switch to the next camera
        if (timer >= switchInterval)
        {
            // Deactivate the currently active camera
            if (orderedCameras[currentCameraIndex] != null)
            {
                orderedCameras[currentCameraIndex].gameObject.SetActive(false);
            }

            // Move to the next camera index and loop back to the start if at the end
            currentCameraIndex = (currentCameraIndex + 1) % orderedCameras.Length;

            // Activate the next camera in the sequence
            if (orderedCameras[currentCameraIndex] != null)
            {
                orderedCameras[currentCameraIndex].gameObject.SetActive(true);
            }

            // Reset the timer for the next switch
            timer = 0.0f;
        }
    }
}