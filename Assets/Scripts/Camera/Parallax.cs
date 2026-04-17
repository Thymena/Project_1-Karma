using UnityEngine;
using System.Reflection;

/// <summary>
/// Parallax effect for individual background layers.
/// Attach this to each background layer GameObject.
/// Works correctly with Cinemachine bounded cameras.
/// </summary>
public class Parallax : MonoBehaviour
{
    [Header("Camera Reference")]
    [Tooltip("The camera to follow. If not assigned, will try to find Main Camera.")]
    public Transform cameraTransform;

    [Header("Parallax Settings")]
    [Tooltip("How fast this layer moves relative to the camera. 0 = no movement, 1 = moves with camera, 0.5 = moves half speed.")]
    [Range(0f, 1f)]
    public float parallaxMultiplier = 0.5f;

    [Tooltip("Enable infinite scrolling (seamless looping).")]
    public bool infiniteScroll = true;

    [Tooltip("Enable vertical parallax effect.")]
    public bool enableVerticalParallax = false;

    [Header("Movement Detection")]
    [Tooltip("Minimum movement distance to trigger parallax. Prevents jitter when camera is bounded.")]
    public float movementThreshold = 0.01f;

    [Header("Infinite Scroll Settings")]
    [Tooltip("Width of the sprite for infinite scrolling. Auto-calculated if sprite renderer exists.")]
    public float spriteWidth = 10f;

    private Vector3 lastCameraPosition;
    private SpriteRenderer spriteRenderer;
    private float textureUnitSizeX;
    private Camera actualCamera;
    private Component virtualCamera; // CinemachineVirtualCamera stored as Component for compatibility
    private System.Type virtualCameraType;
    private PropertyInfo finalPositionProperty;

    void Start()
    {
        // Auto-find camera if not assigned
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraTransform = mainCam.transform;
                actualCamera = mainCam;
            }
            else
            {
                Debug.LogWarning("Parallax: No camera assigned and no Main Camera found!");
                enabled = false;
                return;
            }
        }
        else
        {
            // Get the Camera component from the transform
            actualCamera = cameraTransform.GetComponent<Camera>();
            if (actualCamera == null)
            {
                // If no camera on this transform, try to find Main Camera
                actualCamera = Camera.main;
                if (actualCamera != null)
                {
                    cameraTransform = actualCamera.transform;
                }
            }
        }

        // Try to find Cinemachine virtual camera using reflection (works without direct reference)
        if (actualCamera != null)
        {
            try
            {
                // Get CinemachineVirtualCamera type using reflection
                virtualCameraType = System.Type.GetType("Cinemachine.CinemachineVirtualCamera");
                if (virtualCameraType != null)
                {
                    // Search for virtual camera in parent hierarchy
                    Transform parent = actualCamera.transform.parent;
                    while (parent != null)
                    {
                        virtualCamera = parent.GetComponent(virtualCameraType);
                        if (virtualCamera != null) break;
                        parent = parent.parent;
                    }
                    
                    // If not found in parent, search all virtual cameras
                    if (virtualCamera == null)
                    {
                        var findMethod = typeof(Object).GetMethod("FindObjectsOfType", new System.Type[] { typeof(System.Type) });
                        if (findMethod != null)
                        {
                            var allVCams = findMethod.Invoke(null, new object[] { virtualCameraType }) as Component[];
                            if (allVCams != null && allVCams.Length > 0)
                            {
                                // Try to match by orthographic size
                                var lensProperty = virtualCameraType.GetProperty("m_Lens");
                                if (lensProperty != null)
                                {
                                    foreach (var vcam in allVCams)
                                    {
                                        var lens = lensProperty.GetValue(vcam);
                                        var orthoSizeProperty = lens.GetType().GetProperty("OrthographicSize");
                                        if (orthoSizeProperty != null)
                                        {
                                            float orthoSize = (float)orthoSizeProperty.GetValue(lens);
                                            if (Mathf.Approximately(orthoSize, actualCamera.orthographicSize))
                                            {
                                                virtualCamera = vcam;
                                                break;
                                            }
                                        }
                                    }
                                }
                                
                                // If no match found, use the first one
                                if (virtualCamera == null && allVCams.Length > 0)
                                {
                                    virtualCamera = allVCams[0];
                                }
                            }
                        }
                    }
                    
                    // Get the State property and FinalPosition property for later use
                    if (virtualCamera != null)
                    {
                        var stateProperty = virtualCameraType.GetProperty("State");
                        if (stateProperty != null)
                        {
                            var stateType = stateProperty.PropertyType;
                            finalPositionProperty = stateType.GetProperty("FinalPosition");
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                // Cinemachine not available or error finding it - continue without it
                virtualCamera = null;
            }
        }

        // Initialize with actual camera position (respects Cinemachine bounds)
        lastCameraPosition = GetActualCameraPosition();

        // Calculate sprite width for infinite scrolling
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && spriteRenderer.sprite != null)
        {
            textureUnitSizeX = spriteRenderer.sprite.texture.width / spriteRenderer.sprite.pixelsPerUnit;
            if (spriteWidth <= 0)
            {
                spriteWidth = textureUnitSizeX;
            }
        }
    }

    /// <summary>
    /// Gets the actual rendered camera position, which respects Cinemachine bounds.
    /// </summary>
    Vector3 GetActualCameraPosition()
    {
        // Use Cinemachine's final position if available (this respects all constraints and bounds)
        if (virtualCamera != null && finalPositionProperty != null)
        {
            try
            {
                var stateProperty = virtualCameraType.GetProperty("State");
                if (stateProperty != null)
                {
                    var state = stateProperty.GetValue(virtualCamera);
                    if (state != null)
                    {
                        var finalPos = finalPositionProperty.GetValue(state);
                        if (finalPos is Vector3)
                        {
                            return (Vector3)finalPos;
                        }
                    }
                }
            }
            catch (System.Exception)
            {
                // Fall through to fallback
            }
        }
        
        // Fallback to actual Camera component's transform position
        // This is the position that's actually rendered (respects Cinemachine bounds)
        if (actualCamera != null)
        {
            return actualCamera.transform.position;
        }
        // Fallback to transform position if no camera component found
        return cameraTransform != null ? cameraTransform.position : Vector3.zero;
    }

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Get the actual camera position (this respects Cinemachine bounds)
        Vector3 currentCameraPosition = GetActualCameraPosition();
        Vector3 deltaMovement = currentCameraPosition - lastCameraPosition;
        
        // Only apply parallax if camera actually moved beyond threshold
        // This prevents background movement when camera is bounded or has small jitter
        float movementMagnitude = deltaMovement.magnitude;
        if (movementMagnitude > movementThreshold)
        {
            // Apply parallax effect
            float parallaxX = deltaMovement.x * parallaxMultiplier;
            float parallaxY = enableVerticalParallax ? deltaMovement.y * parallaxMultiplier : 0f;
            
            transform.position += new Vector3(parallaxX, parallaxY, 0f);
            
            // Update last position only if we actually moved
            lastCameraPosition = currentCameraPosition;
        }
        // Don't update lastCameraPosition if movement was below threshold
        // This prevents accumulation of tiny movements

        // Infinite scrolling
        if (infiniteScroll && spriteWidth > 0)
        {
            float distanceFromCamera = currentCameraPosition.x - transform.position.x;
            
            if (Mathf.Abs(distanceFromCamera) >= spriteWidth)
            {
                float offsetPositionX = (distanceFromCamera % spriteWidth);
                transform.position = new Vector3(currentCameraPosition.x + offsetPositionX, transform.position.y, transform.position.z);
            }
        }
    }
}