using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewTrigger : MonoBehaviour
{
    public Material acceptMaterial;
    public Material denyMaterial;
    public LayerMask layerMask;
    public bool canPlace;

    private Renderer render;
    private int colliderCount;

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        colliderCount = 0;
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(0.5f, 0.05f, 0.5f), Quaternion.identity);

        foreach (Collider c in hitColliders)
        {
            if (c.GetComponent<IFreezable>() != null)
                continue;

            if (c.CompareTag("Water") || c.CompareTag("Preview") || c.CompareTag("FreezePushable") || c.gameObject.layer == 2 || c.gameObject.layer == 9)
                continue;

            colliderCount++;
        }

        if (colliderCount <= 0)
        {
            render.material = acceptMaterial;
            canPlace = true;
        }
        else
        {
            render.material = denyMaterial;
            canPlace = false;
        }
    }
}
