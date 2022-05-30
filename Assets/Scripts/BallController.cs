using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class BallController : MonoBehaviour
    {
        public static BallController Instance { get; private set; }

        [SerializeField]
        GameObject mesh;

        Rigidbody rb;
        Collider coll;
        Vector3 defaultPosition;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                rb = GetComponent<Rigidbody>();
                coll = GetComponent<Collider>();
                defaultPosition = rb.position;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Explode()
        {
            // Hide the mesh
            mesh.SetActive(false);

            // Reset the physics
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
            coll.enabled = false;

        }

        public void Reset()
        {
            
            rb.position = defaultPosition;
            coll.enabled = true;
            rb.isKinematic = false;

            mesh.SetActive(true);
        }
    }

}
