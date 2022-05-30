using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    

    

    public class MatchController : MonoBehaviourPunCallbacks
    {
        int matchState;
        double startTime;
        float startDelay = 6;

        // Start is called before the first frame update
        void Start()
        {
            // Get the start time
            startTime = RoomCustomPropertyUtility.GetStartTime(PhotonNetwork.CurrentRoom);
            // Get match state
            matchState = RoomCustomPropertyUtility.GetMatchState(PhotonNetwork.CurrentRoom);

            UpdateMatchState();
                
        }

        // Update is called once per frame
        void Update()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                switch (matchState)
                {
                    case (byte)MatchState.Paused:
                        // Check if the game has started
                        //if (PhotonNetwork.Time - startTime > startDelay)
                        if(PhotonNetwork.Time > startTime)
                        {
                            RoomCustomPropertyUtility.SetMatchState(PhotonNetwork.CurrentRoom, (byte)MatchState.Playing);
                            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
                        }

                        break;
                }
            }

            
        }

        void UpdateMatchState()
        {
            switch (matchState)
            {
                case (byte)MatchState.Playing:
                    PlayerController.LocalPlayerController.SetEnabled(true);
                    break;

                case (byte)MatchState.Completed:
                case (byte)MatchState.Paused:
                    PlayerController.LocalPlayerController.SetEnabled(false);
                    break;
            }
        }


        
        #region pun callbacks
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            if(propertiesThatChanged.ContainsKey(RoomCustomPropertyUtility.StartTimeKey))
            {
                startTime = (double)propertiesThatChanged[RoomCustomPropertyUtility.StartTimeKey];
            }

            if (propertiesThatChanged.ContainsKey(RoomCustomPropertyUtility.MatchStateKey))
            {
                matchState = (byte)propertiesThatChanged[RoomCustomPropertyUtility.MatchStateKey];
                // Update the state
                UpdateMatchState();
            }
        }
        #endregion

    }
}

