using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    /// <summary>
    /// Player controller.
    /// </summary>
    public class PlayerController : MonoBehaviourPunCallbacks, IPunObservable
    {
        [SerializeField]
        float maxSpeed = 3;

        [SerializeField]
        float acc = 5;

        /// <summary>
        /// If > 0 a little more of bouncing to the player when hit things
        /// </summary>
        [SerializeField]
        float extraBounceForce = 0; 

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

            // Store the starting position ( useful on reset )
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

        private void OnCollisionEnter(Collision collision)
        {
           
            if (photonView.IsMine)
            {
                Vector3 forceDir = collision.contacts[0].normal;
                forceDir.y = 0;
                currentVelocity += forceDir * extraBounceForce;
            }
        }

        /// <summary>
        /// Disables the controller ( local player only )
        /// </summary>
        /// <param name="value"></param>
        public void SetEnabled(bool value)
        {
            controllerEnabled = value;
        }

        /// <summary>
        /// Sets/resets collider ( all players )
        /// </summary>
        /// <param name="value"></param>
        public void SetColliderEnabled(bool value)
        {
            coll.enabled = value;
        }

        /// <summary>
        /// Reset on paused ( all players )
        /// </summary>
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
