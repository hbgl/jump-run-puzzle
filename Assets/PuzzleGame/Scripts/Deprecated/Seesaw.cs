using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpRunPuzzle.Multiplayer.TestLab.Scripts
{
    public class Seesaw : MonoBehaviour
    {
        private float maxAngle;         // Maximaler Winkel, in die sich die Wippe nach unten bewegen kann
        public GameObject hitboxLeft;   // Linke Hitbox
        public GameObject hitboxRight;  // Rechte Hitbox
        public int direction = 1;       // Die Richtung, in die sich die Wippe bewegt: 1 = Rechts | -1 = Links

        void Start()
        {
            // Initialisiert die Wippe nach rechts geneigt
            maxAngle = Mathf.Atan((transform.position.y - transform.localScale.y * 0.5f) / (transform.localScale.z * 0.5f)) * Mathf.Rad2Deg;
        }

        void Update()
        {
            // Bewegt sich mit einer Geschwindigkeit in die jeweilige Richtung
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(maxAngle * direction, 0, 0), Time.deltaTime * 10f);
        }

        // Setzt die Kipprichtung
        public void SetDirection(int dir)
        {
            direction = dir;
        }

        //private void OnCollisionStay(Collision collision)
        //{
        //    if(collision.transform.position.z > transform.position.z)
        //    {
        //        transform.rotation = Quaternion.Euler(maxAngle, 0, 0);
        //    } else
        //    {
        //        transform.rotation = Quaternion.Euler(-maxAngle, 0, 0);
        //    }
        //}
    }
}