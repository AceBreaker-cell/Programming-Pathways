using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraBackgroundFit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer backgroundRenderer;
    
    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        FitBackground();
    }

    void FitBackground()
    {
        if (backgroundRenderer == null) return;

        float worldHeight = cam.orthographicSize * 2f;
        float worldWidth = worldHeight * cam.aspect;

        float spriteHeight = backgroundRenderer.sprite.bounds.size.y;
        float spriteWidth = backgroundRenderer.sprite.bounds.size.x;

        float scaleX = worldWidth / spriteWidth;
        float scaleY = worldHeight / spriteHeight;

        // Pakai scale terbesar agar background selalu COVER (tidak ada hitam di tepi)
        float scale = Mathf.Max(scaleX, scaleY);

        backgroundRenderer.transform.localScale = new Vector3(scale, scale, 1f);
    }
}