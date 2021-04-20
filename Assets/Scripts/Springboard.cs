using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Springboard : MonoBehaviour
{
    public Rigidbody player;
    
    [Range(-100f, 100f)]
    public float velocity = -3f;

    public SpringboardMode mode = SpringboardMode.None;

    private List<ContactPoint> Contacts = new List<ContactPoint>();

    private double T;

    private float angle;

    private HingeJoint hingeJoint;

    private bool reset;

    [Range(10f, 500f)]
    public float mass = 25;

    private float origMass;

    // Start is called before the first frame update
    void Start()
    {
        T = Time.timeAsDouble;
        hingeJoint = gameObject.GetComponent<HingeJoint>();
        angle = hingeJoint.angle;
    }

    // Update is called once per frame
    void Update()
    {
        var prevAngle = angle;
        var currAngle = hingeJoint.angle;
     
        if (prevAngle < 0 && currAngle > prevAngle && reset)
        {
            switch (mode)
            {
                case SpringboardMode.Velocity:
                    player.velocity = new Vector3(0, 0, 0);
                    break;
                case SpringboardMode.Mass:
                    player.mass = origMass;
                    break;
            }
            
            reset = false;
            Debug.Log("Exit");
        }

        angle = currAngle;
    }

    private void OnCollisionEnter(Collision collision)
    {
        var t = Time.timeAsDouble;
        var elapsed = t - T;
        if (elapsed >= 0 && collision.impulse.magnitude > 70)
        {
            T = t;
            
            switch (mode)
            {
                case SpringboardMode.Velocity:
                    player.velocity = new Vector3(0, velocity, 0);
                    break;
                case SpringboardMode.Mass:
                    origMass = player.mass;
                    player.mass = mass;
                    break;
            }

            //gameObject.GetComponent<Rigidbody>().AddForceAtPosition(new Vector3(0, force, 0), new Vector3(2.8f, 1f, -48.5f), ForceMode.Impulse);
            
            reset = true;
            Debug.Log($"Hit");
        }
    }
}


public enum SpringboardMode
{
    None,
    Mass,
    Velocity,
}