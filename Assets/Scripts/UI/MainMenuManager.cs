using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis.UI
{
    public class MainMenuManager : MonoBehaviourPunCallbacks
    {

        [SerializeField]
        List<GameObject> panels;

       
        // Start is called before the first frame update
        void Start()
        {
            OpenPanel(0);
        }

        // Update is called once per frame
        void Update()
        {

        }

        void HideAll()
        {
            foreach (GameObject panel in panels)
                panel.SetActive(false);
        }

        void OpenPanel(int id)
        {
            HideAll();
            panels[id].gameObject.SetActive(true);
        }

       

        #region pun callbacks
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.LogFormat("PUN - OnJoinedRoom: {0}", PhotonNetwork.CurrentRoom.Name);
            if(PhotonNetwork.CurrentRoom.PlayerCount == PhotonNetwork.CurrentRoom.MaxPlayers)
                OpenPanel(2); // Room is full, to the lobby
            else
                OpenPanel(1); // Room isn't full, waiting for opponent
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.LogFormat("PUN - OnLeftRoom");
            OpenPanel(0);
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.LogFormat("PUN - OnPlayerLeftRoom");

            OpenPanel(1); // Room is no longer full, waiting for opponent again
        }

        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
            Debug.LogFormat("PUN - OnPlayerEnteredRoom");

            // Playing 1vs1 if a new player entered the room should be full, so we move to the lobby
            OpenPanel(2);
        }

        #endregion
    }

}
