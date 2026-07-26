using UnityEngine;
using System.Collections.Generic;

public class StatueArrow : MonoBehaviour
{
    [Header("References")]
    public Transform[] statues;
    public GameObject arrowObject;
    public Vector3 arrowOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Settings")]
    public float detectionRadius = 1.2f;
    public float fadeSpeed = 4f;

    private Transform player;
    private SpriteRenderer[] arrowRenderers;
    private Transform closestStatue;
    private HashSet<int> completedStatues = new HashSet<int>();

    void Start()
    {
        var p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        arrowRenderers = arrowObject.GetComponentsInChildren<SpriteRenderer>();
        SetAlpha(0f);
    }

    public void AddCompletedStatue(int index)
    {
        completedStatues.Add(index);
    }

    void Update()
    {
        if (player == null) return;

        float closestDist = float.MaxValue;
        closestStatue = null;

        for (int i = 0; i < statues.Length; i++)
        {
            if (completedStatues.Contains(i)) continue;
            if (statues[i] == null) continue;

            float dist = Vector2.Distance(player.position, statues[i].position);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestStatue = statues[i];
            }
        }

        bool isClose = closestStatue != null && closestDist <= detectionRadius;

        float currentAlpha = arrowRenderers[0].color.a;
        float targetAlpha = isClose ? 1f : 0f;
        float newAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, fadeSpeed * Time.deltaTime);
        SetAlpha(newAlpha);

        if (closestStatue != null)
            arrowObject.transform.position = closestStatue.position + arrowOffset;
    }

    void SetAlpha(float alpha)
    {
        foreach (var sr in arrowRenderers)
        {
            Color c = sr.color;
            c.a = alpha;
            sr.color = c;
        }
    }
}