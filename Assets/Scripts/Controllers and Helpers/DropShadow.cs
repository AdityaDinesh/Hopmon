using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropShadow : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform shadowQuad;
    [SerializeField] private MeshRenderer shadowRenderer;

    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxShadowDistance = 10f;
    [SerializeField] private float yOffset = 0.02f; // Prevents z-fighting on platforms

    [Header("Visual Dynamics")]
    [SerializeField] private float baseScale = 8f;
    [SerializeField] private float minScaleMultiplier = 0.4f;

    private Material shadowMaterial;
    private int colorShaderID;
    private Transform _transform;
    private GameObject _shadowQuadGameObject;

    void Start()
    {
        if (shadowRenderer != null)
        {
            // Creates a local instance of the material so it doesn't modify the project asset
            shadowMaterial = shadowRenderer.material;
            colorShaderID = Shader.PropertyToID("_BaseColor"); // Standard URP Unlit property
            _transform = transform;
            _shadowQuadGameObject = shadowQuad.gameObject;
        }
    }

    void LateUpdate()
    {
        if (shadowQuad == null || shadowMaterial == null || !_shadowQuadGameObject.activeInHierarchy) return;

        // Raycast down from character center
        if (Physics.Raycast(_transform.position, Vector3.down, out RaycastHit hit, maxShadowDistance, groundLayer))
        {
            _shadowQuadGameObject.SetActive(true);

            // 1. Position: Snap flat to the platform surface
            shadowQuad.position = hit.point + new Vector3(0, yOffset, 0);

            // 2. Dynamics: Calculate distance percentage (0 at ground, 1 at max height)
            float currentDistance = hit.distance;
            float distanceRatio = Mathf.Clamp01(currentDistance / maxShadowDistance);

            // Inverse ratio: 1 at ground, 0 at maximum height
            float intensity = 1f - distanceRatio;

            // 3. Scale: Shrink the shadow as the player ascends
            float currentScale = baseScale * Mathf.Lerp(minScaleMultiplier, 1f, intensity);
            shadowQuad.localScale = new Vector3(currentScale, currentScale, 1f);

            // 4. Fade: Reduce alpha opacity as player ascends
            Color currentColor = shadowMaterial.GetColor(colorShaderID);
            currentColor.a = intensity;
            shadowMaterial.SetColor(colorShaderID, currentColor);
        }
        else
        {
            // Turn off shadow completely if jumping over a death pit/void
            _shadowQuadGameObject.SetActive(false);
        }
    }

    void OnDestroy()
    {
        // Clean up instantiated material to prevent memory leaks
        if (shadowMaterial != null) Destroy(shadowMaterial);
    }
}
