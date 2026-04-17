using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages multiple parallax layers for easier setup and organization.
/// Attach this to an empty GameObject in your scene (optional).
/// </summary>
public class ParallaxManager : MonoBehaviour
{
    [Header("Camera Reference")]
    [Tooltip("The camera to follow. If not assigned, will use Main Camera.")]
    public Transform cameraTransform;

    [Header("Parallax Layers")]
    [Tooltip("List of all parallax layers. Add your background GameObjects here.")]
    public List<ParallaxLayer> parallaxLayers = new List<ParallaxLayer>();

    [System.Serializable]
    public class ParallaxLayer
    {
        [Tooltip("The GameObject with the Parallax component.")]
        public GameObject layerObject;
        
        [Tooltip("Parallax speed multiplier (0 = no movement, 1 = moves with camera).")]
        [Range(0f, 1f)]
        public float parallaxMultiplier = 0.5f;
        
        [Tooltip("Enable infinite scrolling for this layer.")]
        public bool infiniteScroll = true;
        
        [Tooltip("Enable vertical parallax for this layer.")]
        public bool enableVerticalParallax = false;
    }

    void Start()
    {
        // Auto-find camera if not assigned
        if (cameraTransform == null)
        {
            Camera mainCam = Camera.main;
            if (mainCam != null)
            {
                cameraTransform = mainCam.transform;
            }
        }

        // Setup all parallax layers
        foreach (var layer in parallaxLayers)
        {
            if (layer.layerObject != null)
            {
                Parallax parallax = layer.layerObject.GetComponent<Parallax>();
                if (parallax == null)
                {
                    parallax = layer.layerObject.AddComponent<Parallax>();
                }

                parallax.cameraTransform = cameraTransform;
                parallax.parallaxMultiplier = layer.parallaxMultiplier;
                parallax.infiniteScroll = layer.infiniteScroll;
                parallax.enableVerticalParallax = layer.enableVerticalParallax;
            }
        }
    }

    /// <summary>
    /// Add a new parallax layer at runtime.
    /// </summary>
    public void AddParallaxLayer(GameObject layerObject, float multiplier, bool infiniteScroll = true, bool verticalParallax = false)
    {
        ParallaxLayer newLayer = new ParallaxLayer
        {
            layerObject = layerObject,
            parallaxMultiplier = multiplier,
            infiniteScroll = infiniteScroll,
            enableVerticalParallax = verticalParallax
        };

        parallaxLayers.Add(newLayer);

        Parallax parallax = layerObject.GetComponent<Parallax>();
        if (parallax == null)
        {
            parallax = layerObject.AddComponent<Parallax>();
        }

        parallax.cameraTransform = cameraTransform;
        parallax.parallaxMultiplier = multiplier;
        parallax.infiniteScroll = infiniteScroll;
        parallax.enableVerticalParallax = verticalParallax;
    }
}

