using UnityEngine;

public class ParallaxBackground : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTransform;

    [Header("Parallax Settings")]
    [Range(0f, 1f)]
    public float parallaxSpeed = 0.1f;

    private float spriteWidth;
    private Vector3 lastCameraPos;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraPos = cameraTransform.position;

        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            spriteWidth = sr.bounds.size.x;
    }

    void LateUpdate()
    {
        // Gerak parallax
        float deltaX = cameraTransform.position.x - lastCameraPos.x;
        transform.position += new Vector3(deltaX * parallaxSpeed, 0f, 0f);
        lastCameraPos = cameraTransform.position;

        // Looping kanan
        if (cameraTransform.position.x - transform.position.x > spriteWidth)
            transform.position += new Vector3(spriteWidth, 0f, 0f);

        // Looping kiri
        if (cameraTransform.position.x - transform.position.x < -spriteWidth)
            transform.position -= new Vector3(spriteWidth, 0f, 0f);
    }
}