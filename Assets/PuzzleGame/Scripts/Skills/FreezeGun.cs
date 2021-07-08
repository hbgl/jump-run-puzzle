using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FreezeGun : MonoBehaviour
{
    public float maxDist;
    public LayerMask interactionLayer;
    public GameObject previewPrefab;

    private Animator animator;
    private GameObject preview;
    private bool showPreview;
    private RaycastHit hit;

    private bool wandIsActive;

    // Start is called before the first frame update
    void Start()
    {
        if(GetComponent<PhotonView>().IsMine)
        {
            animator = GetComponent<Animator>();
            preview = Instantiate(previewPrefab);
            preview.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<PhotonView>().IsMine == false)
        { 
            return; 
        }

        showPreview = false;
        wandIsActive = false;

        if (Input.GetKey(KeyCode.Mouse1))
        {
            wandIsActive = true;

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
                PhotonNetwork.Instantiate("IceMain", new Vector3(hit.point.x, hit.point.y, hit.point.z), Quaternion.identity);
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

        animator.SetBool("wandIsActive", wandIsActive);
    }

    private void OnDisable()
    {
        if (preview != null && preview.activeInHierarchy)
        {
            preview.SetActive(false);
        }
    }
}
