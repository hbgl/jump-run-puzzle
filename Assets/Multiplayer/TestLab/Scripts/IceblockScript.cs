using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceblockScript : MonoBehaviour
{

    public Animator animator;
    public GameObject ice;

    private Collider frozenObject;

    // Start is called before the first frame update
    void Start()
    {
        Collider[] hitColliders = Physics.OverlapBox(transform.position, new Vector3(0.5f, 0.5f, 0.5f));
        foreach (Collider c in hitColliders)
        {
            if(c.CompareTag("Freezable"))
            {
                frozenObject = c;
                c.attachedRigidbody.detectCollisions = false;
                c.transform.SetParent(transform);
                c.transform.localPosition = Vector3.zero;
                c.attachedRigidbody.isKinematic = true;
                ice.layer = LayerMask.NameToLayer("Freezable");
                break;
            }
        }
            Invoke("Melt", 8f);
    }

    // Update is called once per frame
    void Update()
    {
        if (frozenObject)
        {
            frozenObject.transform.position = ice.transform.position;
        }
    }

    private void Melt()
    {
        if (frozenObject)
        {
            frozenObject.attachedRigidbody.detectCollisions = true;
            frozenObject.attachedRigidbody.isKinematic = false;
            frozenObject.transform.SetParent(null);
        }
        animator.Play("Melt");
        Destroy(gameObject, animator.GetCurrentAnimatorStateInfo(0).length);
    }
}
