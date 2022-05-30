using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class GameMenuManager : MonoBehaviour
    {
        [SerializeField]
        GameObject panel;

        // Start is called before the first frame update
        void Start()
        {
            panel.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                panel.SetActive(!panel.activeSelf);
            }
        }
    }

}
