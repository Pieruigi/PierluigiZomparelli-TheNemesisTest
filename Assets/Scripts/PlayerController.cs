using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField]
        float maxSpeed = 3;

        [SerializeField]
        float acc = 5;

        Rigidbody rb;
        Vector3 targetVelocity, currentVelocity;

        bool controllerEnabled = false;
        Collider coll;
        Vector3 positionDefault;

        public static PlayerController LocalPlayerController { get; private set; }
        

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
            coll = GetComponent<Collider>();
            positionDefault = rb.position;

            // Set the local player controller static field
            if (photonView.Owner == PhotonNetwork.LocalPlayer)
                LocalPlayerController = this;
        }

      
        // Update is called once per frame
        void Update()
        {
          
            if(photonView.IsMine) // This is the local player
            {
                if (!controllerEnabled)
                    return;

                // Get input
                Vector2 inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                
                // Get desired velocity
                targetVelocity = new Vector3(inputAxis.x, 0, inputAxis.y).normalized * maxSpeed;

                // Compute actual velocity
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, Time.deltaTime * acc);

                

                
            }
            else // Remote player, put your synch code here if you don't want to relay on PUN rb synch
            {
                // Interpolate remote player here if you want and remove rb from the observed component
            }
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                rb.velocity = currentVelocity;
            }
        }

        public void SetEnabled(bool value)
        {
            controllerEnabled = value;
        }

        public void SetColliderEnabled(bool value)
        {
            coll.enabled = value;
        }

        public void Reset()
        {
            SetEnabled(false);
            rb.velocity = Vector3.zero;
            targetVelocity = Vector3.zero;
            currentVelocity = targetVelocity;
            rb.isKinematic = true;
            coll.enabled = false;
            rb.position = positionDefault;
            rb.isKinematic = false;
            coll.enabled = true;
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            // You can send specific data to synch on photon network
            // you can send rb.velocity here and do manual interpolation over network
        }
    }

}
