using UnityEngine;

public class WaterAnimation : MonoBehaviour
{
    [Header("Wave Settings")]
    public float waveSpeed = 1f;       
    public float waveHeight = 0.1f;    
    public float scrollSpeed = 0.5f;   

    private SpriteRenderer sr;
    private Vector3 startPos;
    private Material waterMat;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        startPos = transform.position;

        
        if (sr != null && sr.material != null)
            waterMat = sr.material;
    }

    void Update()
    {
        
        float newY = startPos.y + Mathf.Sin(Time.time * waveSpeed) * waveHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);

        
        if (waterMat != null)
        {
            float offset = Time.time * scrollSpeed;
            waterMat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
        }
    }
}