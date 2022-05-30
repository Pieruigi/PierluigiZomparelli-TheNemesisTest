using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class Launcher : MonoBehaviourPunCallbacks
    {
        
        

        public static Launcher Instance { get; private set; }

        string gameVersion = "1.0";

        
        bool connecting = false;

        int maxPlayers = 2;


        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                // Allow the master client to sync scene to other clients
                PhotonNetwork.AutomaticallySyncScene = true;
                gameVersion = Application.version;

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

        void Connect()
        {
            

            if (PhotonNetwork.IsConnected)
            {
                // Already connected to photon network, join a random room
                Debug.LogFormat("PUN - Connected to Photon network");

                // When the player quits the lobby the client automatically reconnects to the photon network,
                // so this flag avoids the player to rejoin the lobby again
                if (connecting)
                {
                    connecting = false;
                    Debug.Log("PUN - JoinRandomOrCreateRoom()");
                    PhotonNetwork.JoinRandomRoom(null, (byte)maxPlayers);

                }
                

                
            }
            else
            {
                // Connect to the photon network first
                Debug.LogFormat("PUN - Connecting to Photon network...");
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
            }
        }

        #region public methods
        public void JoinRandomRoom()
        {
            
            connecting = true;
            Connect();
        }

        public void Quit()
        {
            connecting = false;
            PhotonNetwork.LeaveRoom();

        }
        #endregion


        #region pun callbacks
        public override void OnConnectedToMaster()
        {
            base.OnConnectedToMaster();
            Debug.Log("PUN - OnConnectedToMaster");

            if (connecting)
            {
                connecting = false;
                Debug.Log("PUN - JoinRandomOrCreateRoom()");
                PhotonNetwork.JoinRandomRoom(null, (byte)maxPlayers);
            }
            

        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            // No room found
            base.OnJoinRandomFailed(returnCode, message);
            Debug.LogWarningFormat("PUN - OnJoinRandomFailed:({0}) {1}", returnCode, message);

            // Create a new room
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte)maxPlayers;
            PhotonNetwork.CreateRoom(null, options, null);
        }

        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.LogFormat("PUN - OnJoinedRoom: {0}", PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnCreatedRoom()
        {
            base.OnCreatedRoom();
            Debug.LogFormat("PUN - OnCreatedRoom: {0}", PhotonNetwork.CurrentRoom.Name);
        }

        public override void OnLeftRoom()
        {
            base.OnLeftRoom();
            Debug.LogFormat("PUN - OnLeftRoom");

        }

        public override void OnDisconnected(DisconnectCause cause)
        {
            base.OnDisconnected(cause);
            Debug.LogFormat("PUN - OnDisconnected: {0}", cause.ToString());
        }
        
        #endregion
    }

}
