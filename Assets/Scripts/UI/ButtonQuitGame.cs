using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TheNemesis
{
    public class ButtonQuitGame : MonoBehaviour
    {

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(()=> { PhotonNetwork.LeaveRoom(); });
        }

      
    }

}
