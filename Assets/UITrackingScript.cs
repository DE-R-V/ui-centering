using System;
using UnityEngine;
using UnityEngine.UI;

public class UITrackingScript : MonoBehaviour
{
    [Header("GameObjects & Components")]
    [SerializeField]
    [Tooltip("The UI Camera used for centering calculations")]
    private Camera camera;

    [SerializeField]
    [Tooltip("The GameObject that follows the camera")]
    private GameObject objectThatFollows;

    [SerializeField]
    [Tooltip("Toggle button to enable/disable the tracking behavior, tracking is enabled by default")]
    private Button toggleButton;
    private bool isTrackingEnabled = true;

    [SerializeField]
    [Tooltip("Z axis offset from the camera position. Use negative values to move closer to the camera.\n Suggested values:\n - Between 0.45 and 0.55 is suggested for poke interactions\n - At least 0.7 is suggested for ray interactions")]
    private float zOffset;


    void Awake()
    {
        objectThatFollows = this.gameObject;
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleTracking);
        }
    }

    void Start()
    {
        if (camera == null)
        {
            // Find the first available camera in the scene
            camera = FindObjectsByType<Camera>(FindObjectsSortMode.None)[0];
        }
    }

    void Update()
    {
        if (isTrackingEnabled)
        {
            TrackCamera();
        }
    }

    void TrackCamera()
    {
        if (toggleButton != null)
        {
            UnityEngine.Vector3 cameraPositionToTrack = camera.transform.position;
            cameraPositionToTrack.z += zOffset;
            objectThatFollows.transform.position = cameraPositionToTrack;
        }
    }

    public void ToggleTracking()
    {
        SetTracking(!isTrackingEnabled);
    }

    public void SetTracking(bool newStatus)
    {
        isTrackingEnabled = newStatus;
    }

    void OnDestroy()
    {
        if (toggleButton != null)
        {
            toggleButton.onClick.RemoveListener(ToggleTracking);
        }
    }
}

