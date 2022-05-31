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

      
    }

}
