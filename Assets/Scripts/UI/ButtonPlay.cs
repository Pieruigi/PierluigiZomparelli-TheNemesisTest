using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheNemesis.UI
{
    public class ButtonPlay : MonoBehaviour
    {
        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(() => { Launcher.Instance.JoinRandomRoom(); });
        }

      
    }

}
