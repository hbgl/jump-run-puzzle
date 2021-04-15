using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FreezeGun : MonoBehaviourPun
{
    public float maxDist;
    public LayerMask interactionLayer;
    public GameObject previewPrefab;
    public GameObject iceblockPrefab;

    private GameObject preview;
    private bool showPreview;
    private RaycastHit hit;

    // Start is called before the first frame update
    void Start()
    {
        preview = Instantiate(previewPrefab);
        preview.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        showPreview = false;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            Ray ray = Camera.main.ViewportPointToRay(Vector3.one * 0.5f);
            Debug.DrawRay(ray.origin, ray.direction * maxDist, Color.green);
            if (Physics.Raycast(ray, out hit, maxDist, interactionLayer))
            {
                showPreview = true;
                preview.transform.position = hit.point;
            }
        }

        if(Input.GetKeyUp(KeyCode.Mouse1))
        {
            if(preview.activeInHierarchy && preview.GetComponent<PreviewTrigger>().canPlace)
            {
                Instantiate(iceblockPrefab, new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity);
            }
        }

        if(showPreview)
        {
            if (!preview.activeInHierarchy)
            {
                preview.SetActive(true);
            }
        } else
        {
            if (preview.activeInHierarchy)
            {
                preview.SetActive(false);
            }
        }
    }
}
