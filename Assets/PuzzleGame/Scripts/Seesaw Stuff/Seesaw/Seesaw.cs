using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

namespace JumpRunPuzzle.Seesaw
{
    public class Seesaw : MonoBehaviourPun
    {
        [Range(0f, 1000f)]
        public float EngeryMin = 100f;

        [Range(0f, 10000f)]
        public float EnergyLostFixed = 100f;
        
        [Range(0f, 100f)]
        public float EnergyEfficiency = 100.0f;

        private SeesawPad PadA;
        private SeesawPad PadB;

        private SeesawPad KnockUpPad;
        private float KnockUpEnergy;

        private void Start()
        {
            PadA = transform.GetChild(0).GetComponent<SeesawPad>();
            PadB = transform.GetChild(1).GetComponent<SeesawPad>();
        }

        private void Update()
        {
            if(photonView.IsMine == false)
            {
                return;
            } 

            var energy = PadA.Energy - PadB.Energy;
            var energyAbs = Mathf.Abs(energy);
            var energyEffective = energyAbs * (EnergyEfficiency / 100.0f) - EnergyLostFixed;
            if (energyEffective >= EngeryMin)
            {
                var paddle = energy < 0 ? PadA : PadB;
                KnockUpPad = paddle;
                KnockUpEnergy = energyEffective;
            }
            PadA.Energy = 0f;
            PadB.Energy = 0f;
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine == false)
            {
                return;
            }

            if (KnockUpPad != null)
            {
                KnockEmUpThem(KnockUpPad, KnockUpEnergy);
                KnockUpPad = null;
                KnockUpEnergy = 0;
            }
        }

        private void KnockEmUpThem(SeesawPad paddle, float energy)
        {
            var totalMass = 0f;
            var count = paddle.Touchers.Count;
            for (var i = 0; i < count; i++)
            {
                var toucher = paddle.Touchers[i];
                if (toucher is Rigidbody r)
                {
                    totalMass += r.mass;
                }
                else if (toucher is PhotonView p)
                {
                    totalMass += p.GetComponent<ISeesawAgent>().GetMass();
                }
            }
            if (totalMass <= 0)
            {
                return;
            }
            // E = 0.5 * m * v * v
            // Mathf.Sqrt((2 * E) / m) = v

            var velocity = Mathf.Sqrt(energy * 2 / totalMass);

            for (var i = 0; i < count; i++)
            {
                var toucher = paddle.Touchers[i];
                if (toucher is Rigidbody r)
                {
                    photonView.RPC("SetVelocity", RpcTarget.All, r.GetComponent<PhotonView>().ViewID, velocity);
                    //r.velocity = new Vector3(0f, velocity, 0f);
                }
                else if (toucher is PhotonView p)
                {
                    //p.GetComponent<ISeesawAgent>().SetVelocity(velocity);
                    p.RPC("SetVelocity", RpcTarget.All, velocity);
                }
            }
        }

        [PunRPC]
        public void SetVelocity(int viewID, float velocity)
        {
            PhotonView pv = PhotonNetwork.GetPhotonView(viewID);
            //if(pv.IsMine)
            //{
                pv.GetComponent<Rigidbody>().velocity = new Vector3(0f, velocity, 0f);
            //}
        }
    }
}