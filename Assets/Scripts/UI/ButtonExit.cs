using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheNemesis.UI
{
    public class ButtonExit : MonoBehaviour
    {

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => { Application.Quit(); });
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
