using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailComponent : MonoBehaviour
{

    private TrailRenderer trailRenderer;
    private float duration;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            duration = Random.Range(0.2f, 0.5f);
            timer = 0f;
            trailRenderer.emitting = !trailRenderer.emitting;
        }
    }
}
