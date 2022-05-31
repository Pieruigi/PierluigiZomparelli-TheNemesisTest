using ExitGames.Client.Photon;
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
        GoalArea localGoalArea;
        GoalArea opponentGoalArea;

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;
                PhotonNetwork.NetworkingClient.EventReceived += OnEvent;
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

            // Get the local goal area
            localGoalArea = new List<GoalArea>(GameObject.FindObjectsOfType<GoalArea>()).Find(g => g.IsLocalGoalArea());
            opponentGoalArea = new List<GoalArea>(GameObject.FindObjectsOfType<GoalArea>()).Find(g => !g.IsLocalGoalArea());


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

        private void OnDestroy()
        {
            PhotonNetwork.NetworkingClient.EventReceived -= OnEvent;
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
                    // Set goal area position ( the event is cached, so even the client who enters the second
                    // will receive it ); using events we don't need to add photonView to the goal area
                    ResetGoalAreaPosition();
                    break;
            }
        }

        void ResetGoalAreaPosition()
        {
            // Get a random position
            Vector2 hBound, vBound;

            LevelManager.Instance.GetRandomBoundaries(out hBound, out vBound);
            

            Vector2 newPosition = new Vector2(Random.Range(hBound.x,hBound.y), Random.Range(vBound.x,vBound.y));

            // Set the local goal area
            UpdateGoalAreaPosition(localGoalArea.transform, newPosition);

            // Raise event to update the remote client
            object[] content = new object[] { newPosition }; // Array contains the target position
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; 
            PhotonNetwork.RaiseEvent(EventCode.ResetGoalArea, content, raiseEventOptions, SendOptions.SendReliable);
        }

        void UpdateGoalAreaPosition(Transform target, Vector2 newPosition)
        {
            float defaultY = target.position.y;
            target.position = new Vector3(newPosition.x, defaultY, newPosition.y);
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

        private void OnEvent(EventData photonEvent)
        {
            byte eventCode = photonEvent.Code;
            if (eventCode == EventCode.ResetGoalArea)
            {
                
                object[] data = (object[])photonEvent.CustomData;
                Vector2 targetPosition = (Vector2)data[0];
                // We set the opponent goal area position
                UpdateGoalAreaPosition(opponentGoalArea.transform, targetPosition);
            }
        }
        #endregion

    }


}

