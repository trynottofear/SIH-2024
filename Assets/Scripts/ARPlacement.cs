using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class ARPlacement : MonoBehaviour
{
    public List<GameObject> arObjectsToSpawn; // List to hold multiple prefabs
    public GameObject placementIndicator;
    private GameObject spawnedObject;
    private Pose PlacementPose;
    private ARRaycastManager aRRaycastManager;
    private bool placementPoseIsValid = false;
    public Camera arCamera;
    public GameObject uiPanel;
    public GameObject scalePanel; // Reference to the scale panel UI

    public float scaleIncrement = 0.1f; // Amount by which to scale the object each time
    private Vector3 originalScale; // Store the original scale of the spawned object

    private GameObject selectedPrefab; // Prefab selected to spawn


    public float rotationSpeed = 0.2f; // Speed of rotation for swipe

    private Vector2 initialTouchPosition; // For storing touch start position
    private bool isRotating = false; // Flag to check if user is rotating the object

    public GameObject xrOrigin;

    // Reference to the ARTrackedImageManager
    private ARTrackedImageManager arTrackedImageManager;

    // Boolean to keep track of the enabled/disabled state
    private bool isARImageTrackingEnabled = true;

    void Start()
    {
        aRRaycastManager = FindObjectOfType<ARRaycastManager>();

        // Ensure the scale panel is hidden at the start
        if (scalePanel != null)
        {
            scalePanel.SetActive(false);
        }

        // Get the ARTrackedImageManager component from the XR Origin
        arTrackedImageManager = xrOrigin.GetComponent<ARTrackedImageManager>();

        if (arTrackedImageManager == null)
        {
            Debug.LogError("ARTrackedImageManager component not found on XR Origin!");
        }
    }

    void Update()
    {
        if (spawnedObject == null && placementPoseIsValid && selectedPrefab != null)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
        UpdatePlacementPose();
        UpdatePlacementIndicator();

        if (spawnedObject == null && placementPoseIsValid && selectedPrefab != null)
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                ARPlaceObject();  // Place the object on the screen when user taps
            }
        }


        // Handle rotation with touch input, but avoid UI interactions
        HandleRotation();
    }

    void UpdatePlacementIndicator()
    {
        if (spawnedObject == null && placementPoseIsValid)
        {
            placementIndicator.SetActive(true);
            placementIndicator.transform.SetPositionAndRotation(PlacementPose.position, PlacementPose.rotation);
        }
        else
        {
            placementIndicator.SetActive(false);
        }
    }

    void UpdatePlacementPose()
    {
        var screenCenter = arCamera.ViewportToScreenPoint(new Vector3(0.5f, 0.5f));
        var hits = new List<ARRaycastHit>();
        aRRaycastManager.Raycast(screenCenter, hits, TrackableType.Planes);

        placementPoseIsValid = hits.Count > 0;
        if (placementPoseIsValid)
        {
            PlacementPose = hits[0].pose;
        }
    }

    void ARPlaceObject()
    {
        spawnedObject = Instantiate(selectedPrefab, PlacementPose.position, PlacementPose.rotation);
        originalScale = spawnedObject.transform.localScale;  // Store the original scale

        // Activate the scale panel once the object is spawned
        if (scalePanel != null)
        {
            scalePanel.SetActive(true);
        }
    }

    // Method to be called when button is clicked to switch prefab
    public void SelectPrefab(int prefabIndex)
    {
        if (prefabIndex >= 0 && prefabIndex < arObjectsToSpawn.Count)
        {
            selectedPrefab = arObjectsToSpawn[prefabIndex];
            if (uiPanel != null)
            {
                uiPanel.SetActive(false); // Hide the UI panel
            }
        }
    }

    //public void ScanQR()
    //{
    //    // Toggle the script enabled/disabled state
    //    isARImageTrackingEnabled = !isARImageTrackingEnabled;

    //    if (uiPanel != null)
    //    {
    //        uiPanel.SetActive(false); // Hide the UI panel
    //    }

    //    if (arTrackedImageManager != null)
    //    {
    //        arTrackedImageManager.enabled = isARImageTrackingEnabled;

    //        Debug.Log("ARTrackedImageManager is now set to: " + isARImageTrackingEnabled);
    //    }
    //}

    // Method to scale up the object
    public void ScaleUp()
    {
        if (spawnedObject != null)
        {
            // Scale uniformly in all axes (X, Y, Z)
            Vector3 newScale = spawnedObject.transform.localScale + Vector3.one * scaleIncrement;
            AdjustScale(newScale); // Keep object on the ground
        }
    }

    // Method to scale down the object
    public void ScaleDown()
    {
        if (spawnedObject != null)
        {
            // Scale uniformly in all axes (X, Y, Z)
            Vector3 newScale = spawnedObject.transform.localScale - Vector3.one * scaleIncrement;

            // Ensure that the scale doesn't become negative or too small
            newScale = ClampScale(newScale);

            AdjustScale(newScale); // Keep object on the ground
        }
    }

    // Adjusts the scale and ensures the base stays fixed
    void AdjustScale(Vector3 newScale)
    {
        Vector3 oldScale = spawnedObject.transform.localScale;
        spawnedObject.transform.localScale = newScale;

        // Calculate the height difference after scaling
        float heightDiff = (newScale.y - oldScale.y) * 0.5f;

        // Adjust the position so that the base remains fixed (modify only the Y-axis)
        spawnedObject.transform.position = new Vector3(
            spawnedObject.transform.position.x,
            spawnedObject.transform.position.y + heightDiff,
            spawnedObject.transform.position.z
        );
    }

    // Method to clamp the scale and prevent it from going too small or negative
    Vector3 ClampScale(Vector3 scale)
    {
        // Set a minimum threshold for scale (0.1 in this case)
        float minScale = 0.1f;

        return new Vector3(
            Mathf.Max(scale.x, minScale),
            Mathf.Max(scale.y, minScale),
            Mathf.Max(scale.z, minScale)
        );
    }

    // Handle rotation based on user swipe
    void HandleRotation()
    {
        if (spawnedObject != null)
        {
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);

                // Check if the touch is over a UI element before allowing rotation
                if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                {
                    return; // If touch is on UI, do nothing
                }

                if (touch.phase == TouchPhase.Began)
                {
                    initialTouchPosition = touch.position; // Capture the initial touch position
                    isRotating = true;
                }
                else if (touch.phase == TouchPhase.Moved && isRotating)
                {
                    Vector2 currentTouchPosition = touch.position;
                    float deltaX = currentTouchPosition.x - initialTouchPosition.x;

                    // Rotate the object based on the horizontal swipe movement (deltaX)
                    spawnedObject.transform.Rotate(0, -deltaX * rotationSpeed, 0);

                    // Update the initial touch position for continuous rotation
                    initialTouchPosition = currentTouchPosition;
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isRotating = false;
                }
            }
        }
    }

}
