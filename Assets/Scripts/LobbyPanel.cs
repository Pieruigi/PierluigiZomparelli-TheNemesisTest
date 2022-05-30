using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis.UI
{
    public class LobbyPanel : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        GameObject localPlayer;

        [SerializeField]
        GameObject remotePlayer;

        [SerializeField]
        List<Transform> teamPositions;

        
        Player opponent;
        Vector3 localPlayerDefaultPosition, remotePlayerDefaultPosition;

        private void Awake()
        {
            // Get default positions
            localPlayerDefaultPosition = localPlayer.transform.position;
            remotePlayerDefaultPosition = remotePlayer.transform.position;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public override void OnEnable()
        {
            base.OnEnable();

            if (PhotonNetwork.CurrentRoom == null)
                return;

            // Reset positions
            localPlayer.transform.position = localPlayerDefaultPosition;
            remotePlayer.transform.position = remotePlayerDefaultPosition;

            // Try to get local player team 
            //int localTeamId = PlayerCustomPropertyUtility.GetTeamId(PhotonNetwork.LocalPlayer);
            //object value;

            //if(PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerCustomProperties.TeamKey, out value))
            //{
            //    localTeamId = (byte)value;
            //}

            // Set the opponent field
           
            opponent = new List<Player>(PhotonNetwork.CurrentRoom.Players.Values).Find(p => p != PhotonNetwork.LocalPlayer);

            // Try to get the opponent team
            //int remoteTeamId = PlayerCustomPropertyUtility.GetTeamId(opponent);
            //if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(PlayerCustomProperties.TeamKey, out value))
            //{
            //    remoteTeamId = (byte)value;
            //}
           
            
        }

        public override void OnDisable()
        {
            base.OnDisable();
        }

        #region public methods
        /// <summary>
        /// Used by the local player to join a team
        /// </summary>
        /// <param name="teamId"></param>
        public void TryJoinTeam(int teamId)
        {
            // If the opponent already belongs to the same team do nothing
            if (PlayerCustomPropertyUtility.GetTeamId(opponent) == teamId)
                return;

            // Set the property
            PlayerCustomPropertyUtility.SetTeamId(PhotonNetwork.LocalPlayer, (byte)teamId);
            // Synch on network
            PhotonNetwork.LocalPlayer.SetCustomProperties(PhotonNetwork.LocalPlayer.CustomProperties);
        }
        #endregion

        #region pun callbacks
        public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
        {
            base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);

            // Getting updates when player changes properties ( ex. switching team )
            if(changedProps.ContainsKey(PlayerCustomPropertyUtility.TeamKey))
            {
                // Set the team
                int teamId = (byte)changedProps[PlayerCustomPropertyUtility.TeamKey];
                int offset = 0;
                if(teamId == 2)
                    offset = 1;
               

                if (targetPlayer == PhotonNetwork.LocalPlayer)
                {
                    // Local player
                    localPlayer.transform.position = teamPositions[0+offset].position;
                }
                else
                {
                    // Opponent
                    remotePlayer.transform.position = teamPositions[2+offset].position;
                }
            }
        }
        #endregion
    }

}
