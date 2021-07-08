using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using JumpRunPuzzle.Support;
using Photon.Pun;

namespace JumpRunPuzzle.Seesaw
{
    /**
     * The SeesawPad handles collisions with rigidbodies autimatically.
     * For non-rigidbodies the collision must be handled from the colliding object.
     */
    // TODO: Add photonView to each SeesawPad
    public class SeesawPad : MonoBehaviourPun
    {
        /// <summary>
        /// When an object hits the paddle too slowly then we do
        /// not count the energy. This is particularily useful for
        /// calculating the energy in the OnCollisionStay method
        /// because it reports very low velocities in equilibrium.
        /// </summary>
        private const float PaddleHitVelocityMin = 0.5f;

        public float Energy = 0f;
        public List<object> Touchers = new List<object>(16);

        private Collider Collider;

        private void Start()
        {
            //Seesaw = transform.parent.GetComponent<Seesaw>();
            Collider = GetComponent<Collider>();
        }

        // TODO: Sync Touchers and Energy on collisions with rigidbodies
        private void OnCollisionEnter(Collision collision)
        {
            // Can only handle rigidbodies.
            if (collision.rigidbody == null)
            {
                return;
            }

            // Check if paddle is hit on top.
            if (IsInBounds(collision.collider.bounds))
            {
                Touchers.Add(collision.rigidbody);
                if(photonView.IsMine)
                {
                    foreach(object toucher in Touchers)
                    {
                        Debug.Log(toucher.GetType());
                    }
                }

                // Check if there is collision energy on a remote client caused by a remote object
                float calculatedEnergy = CalculateCollisionEnergy(collision);
                if (photonView.IsMine == false && calculatedEnergy > 0)
                {
                    photonView.RPC(nameof(AddEnergy), RpcTarget.Others, calculatedEnergy);
                }
                else
                {
                    Energy += calculatedEnergy;
                }
            }
        }

        private void OnCollisionStay(Collision collision)
        {
            // Can only handle rigidbodies.
            if (collision.rigidbody == null)
            {
                return;
            }
            // Remove if not hitting the top of the paddle.
            if (!IsInBounds(collision.collider.bounds))
            {
                //photonView.RPC("RemoveAgent", RpcTarget.All, collision.rigidbody.gameObject.GetPhotonView().ViewID);
                Touchers.SwapRemove(collision.rigidbody);
            }
            else
            {
                Touchers.AddIfNotExists(collision.rigidbody);

                // Check if there is collision energy on a remote client caused by a remote object
                float calculatedEnergy = CalculateCollisionEnergy(collision);
                if (photonView.IsMine == false && calculatedEnergy > 0)
                {
                    photonView.RPC(nameof(AddEnergy), RpcTarget.Others, calculatedEnergy);
                }
                else
                {
                    Energy += calculatedEnergy;
                }
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            // Can only handle rigidbodies.
            if (collision.rigidbody == null || photonView.IsMine == false)
            {
                return;
            }
            Touchers.SwapRemove(collision.rigidbody);
        }

        // Add Object to toucher array manually, if object is non-rigidbody
        [PunRPC]
        public void AddAgent(int agentID)
        {
            Touchers.Add(PhotonNetwork.GetPhotonView(agentID));
        }

        // Remove object from toucher array manually, if object is non-rigidbody
        [PunRPC]
        public bool RemoveAgent(int agentID)
        {
            return Touchers.SwapRemove(PhotonNetwork.GetPhotonView(agentID));
        }

        // Calculate energy manually, if object is non-rigidbody
        [PunRPC]
        public void AddEnergy(float energy)
        {
            Energy += energy;
        }

        public bool IsInBounds(Bounds bounds)
        {
            return BoundsUtils.IsInPadBounds(Collider.bounds, bounds);
        }

        private static float CalculateCollisionEnergy(Collision collision)
        {
            if(collision.transform.GetComponent<PhotonView>().IsMine == false)
            {
                return 0;
            }

            var velocity = Mathf.Abs(Mathf.Max(collision.impulse.y / collision.rigidbody.mass, 0f));
            if (velocity < PaddleHitVelocityMin)
            {
                return 0f;
            }
            var energy = 0.5f * collision.rigidbody.mass * velocity * velocity;
            if(energy > 0)
            {
                Debug.LogFormat("<color=#42aaf5>>>>>>>>>>>>>>>>> Calculated Energy: " + energy + "</color>");
            }
            return energy;
        }
    }
}