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

    // Start is called before the first frame update
    void Start()
    {
        render = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(0.5f, 0.1f, 0.5f), Quaternion.identity, ~layerMask);
        if (hitColliders.Length == 0)
        {
            foreach(Collider c in hitColliders)
            {
                Debug.Log(c.name);
            }
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
