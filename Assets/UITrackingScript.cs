using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

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
    [Tooltip("This UI needs a collider to work properly")]
    private BoxCollider uiCollider;

    [Header("Tracking Settings")]
    [SerializeField]
    [Tooltip("Z axis offset from the camera position. Use negative values to move closer to the camera.\n Suggested values:\n - Between 0.45 and 0.55 is suggested for poke interactions\n - At least 0.7 is suggested for ray interactions")]
    private float zOffset;
    [SerializeField]
    private float yOffset;

    [SerializeField]
    [Range(0.01f, 10f)]
    private float smoothness = 0.1f;
    private Vector3 velocity = Vector3.zero;

    void Awake()
    {
        objectThatFollows = this.gameObject;
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(ToggleTracking);
        }
        if (uiCollider == null)
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            MatchColliderToRectTransform(
                gameObject.GetComponent<RectTransform>(),
                collider
            );
            collider.isTrigger = true;
            uiCollider = collider;
            XRSimpleInteractable xrInteractable = this.gameObject.GetComponent<XRSimpleInteractable>();
            xrInteractable.colliders.Add(uiCollider);
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

    void MatchColliderToRectTransform(RectTransform rect, BoxCollider box)
    {
        if (!rect || !box) return;

        Rect r = rect.rect;

        box.size = new Vector3(r.width, r.height, 0.01f);
        box.center = new Vector3(
            r.center.x,
            r.center.y,
            0f
        );
    }


    void TrackCamera()
    {
        if (toggleButton != null)
        {
            Vector3 targetPosition = camera.transform.TransformPoint(new Vector3(0, yOffset, zOffset));
            objectThatFollows.transform.position = Vector3.SmoothDamp(objectThatFollows.transform.position, targetPosition, ref velocity, smoothness);
            Vector3 directionToCamera = camera.transform.position - objectThatFollows.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(directionToCamera);
            var adjustedDirection = Quaternion.Euler(-targetRotation.eulerAngles.x, targetRotation.eulerAngles.y - 180, targetRotation.eulerAngles.z);
            objectThatFollows.transform.rotation = adjustedDirection;
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

