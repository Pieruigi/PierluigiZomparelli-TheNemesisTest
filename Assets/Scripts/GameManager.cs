using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        bool inGame = false;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
         
                DontDestroyOnLoad(gameObject);
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
    }

}
