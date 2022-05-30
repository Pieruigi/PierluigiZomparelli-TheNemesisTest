#define RB
#if !RB
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

        CharacterController cc;
        Vector3 targetVelocity, currentVelocity;
        

        private void Awake()
        {
            cc = GetComponent<CharacterController>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(photonView.IsMine) // This is the local player
            {
                // Get input
                Vector2 inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                Debug.Log("InputAxis:" + inputAxis);

                // Get desired velocity
                targetVelocity = new Vector3(inputAxis.x, 0, inputAxis.y).normalized * maxSpeed;

                // Compute actual velocity
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, Time.deltaTime * acc);

                Debug.Log("Velocity:" + currentVelocity);

                // Move player
                cc.Move(currentVelocity * Time.deltaTime);
            }
            else // Remote player, just synch
            {

            }
        }

        

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            throw new System.NotImplementedException();
        }
    }

}
#endif
#if RB
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
        

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(photonView.IsMine) // This is the local player
            {
                // Get input
                Vector2 inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                Debug.Log("InputAxis:" + inputAxis);

                // Get desired velocity
                targetVelocity = new Vector3(inputAxis.x, 0, inputAxis.y).normalized * maxSpeed;

                // Compute actual velocity
                currentVelocity = Vector3.MoveTowards(currentVelocity, targetVelocity, Time.deltaTime * acc);

                Debug.Log("Velocity:" + currentVelocity);

                
            }
            else // Remote player, just synch
            {

            }
        }

        private void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                rb.velocity = currentVelocity;
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            //throw new System.NotImplementedException();
        }
    }

}
#endif