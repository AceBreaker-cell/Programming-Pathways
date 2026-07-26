using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Follow Settings")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 2f, -10f);

    [Header("Camera Lock")]
    public float minY = 0.124f;

    void LateUpdate()
    {
        if (player == null) return;

        float targetX = player.position.x + offset.x;
        float targetY = Mathf.Max(player.position.y + offset.y, minY);
        float targetZ = offset.z;

        Vector3 targetPos = new Vector3(targetX, targetY, targetZ);
        transform.position = Vector3.Lerp(transform.position, targetPos, smoothSpeed * Time.deltaTime);
    }
}