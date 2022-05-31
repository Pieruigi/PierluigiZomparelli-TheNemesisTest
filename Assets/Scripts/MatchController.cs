using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TheNemesis
{
    

    

    public class MatchController : MonoBehaviourPunCallbacks
    {
        public UnityAction OnOpponentQuit;

        public static MatchController Instance { get; private set; }

        public int State
        {
            get { return matchState; }
        }
        int matchState;
        double startTime;
        float startDelay = 6;

        
        bool opponentQuit = false;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

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

                case (byte)MatchState.Paused:
                    PlayerController.LocalPlayerController.SetEnabled(false);
                    break;
            }
        }

        #region public static fields
        public static void SetMatchCompleted()
        {
            RoomCustomPropertyUtility.SetMatchState(PhotonNetwork.CurrentRoom, (byte)MatchState.Completed);
        }

        public static void SetMatchPaused()
        {
            RoomCustomPropertyUtility.SetMatchState(PhotonNetwork.CurrentRoom, (byte)MatchState.Paused);
            RoomCustomPropertyUtility.SetStartTime(PhotonNetwork.CurrentRoom, PhotonNetwork.Time + Constants.StartDelay);
        }
        #endregion


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

        // If the other player leaves the room you win
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);

            if(matchState != (byte)MatchState.Completed)
            {
                opponentQuit = true;
                RoomCustomPropertyUtility.SetMatchState(PhotonNetwork.CurrentRoom, (byte)MatchState.Completed);
                PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
                OnOpponentQuit?.Invoke();
            }
        }
        #endregion

    }


}

