using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    /// <summary>
    /// Use the launcher to connect to photon network, to join or create rooms.
    /// </summary>
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

      
        void Connect()
        {
            
            if (PhotonNetwork.IsConnected)
            {
                // Already connected to photon network
                Debug.LogFormat("PUN - Connected to Photon network");

                // When the player quits an existing room the client automatically reconnects to the photon 
                // network, so this flag avoids the player to rejoin the lobby again
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
        /// <summary>
        /// Join a random room
        /// </summary>
        public void JoinRandomRoom()
        {
            // First connect
            connecting = true;
            Connect();
        }

        /// <summary>
        /// Quit a room ( whether in game or lobby )
        /// </summary>
        public void Quit()
        {
            // Quit
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
                // Try to join a random room if exists
                PhotonNetwork.JoinRandomRoom(null, (byte)maxPlayers);
            }
            

        }

        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            // No random room has been found, we must create our own room then
            base.OnJoinRandomFailed(returnCode, message);
            Debug.LogWarningFormat("PUN - OnJoinRandomFailed:({0}) {1}", returnCode, message);

            // Create a new room
            RoomOptions options = new RoomOptions();
            options.MaxPlayers = (byte)maxPlayers;
            PhotonNetwork.CreateRoom(null, options, null);
        }

        /// <summary>
        /// When the local player joins a room
        /// </summary>
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();
            Debug.LogFormat("PUN - OnJoinedRoom: {0}", PhotonNetwork.CurrentRoom.Name);
        }

        /// <summary>
        /// On room creation
        /// </summary>
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
