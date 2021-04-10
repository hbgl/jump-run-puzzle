using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonController : MonoBehaviour
{
    public float speed = 100.0f;
    public Rigidbody player;
    public Cinemachine.CinemachineFreeLook camera;

    private void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(h, 0, v).normalized;

        if (direction.magnitude <= 0.1f)
        {
            direction = Vector3.zero;
        }
        else
        {
            var b = 1;
        }
        var x = direction.x * speed;
        var z = direction.z * speed;
        var y = Mathf.Min(player.velocity.y, 0f);
        Vector3 velocity = new Vector3(x, y, z);
        player.velocity = velocity;
    }
}
