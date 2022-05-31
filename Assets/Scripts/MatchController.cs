using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TheNemesis
{
    
    /// <summary>
    /// Takes care of the state of the match ( is playing, in paused or completed ).
    /// The match state and other params such as the StartTime are stored in the roomCustomProperties and can
    /// be only updated by the master client.
    /// </summary>
   
    public class MatchController : MonoBehaviourPunCallbacks
    {
        // Call when the opponent quit the game
        public UnityAction OnOpponentQuit;

        public static MatchController Instance { get; private set; }

        public int State
        {
            get { return matchState; }
        }
        int matchState; 
        double startTime;
        float startDelay = 6; // How much time we must wait for the match to start ( even after each goal )

        
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

            // Get the local goal area.
            // Local goal areas are built in the scene because we don't really need to spawn them, but we 
            // only need to reset the position and this can be done using events.
            localGoalArea = new List<GoalArea>(GameObject.FindObjectsOfType<GoalArea>()).Find(g => g.IsLocalGoalArea());
            opponentGoalArea = new List<GoalArea>(GameObject.FindObjectsOfType<GoalArea>()).Find(g => !g.IsLocalGoalArea());

            // Update state
            UpdateMatchState();
                
        }

        // Update is called once per frame
        void Update()
        {
            // Only the master client can update the state on the room custom properties
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

        /// <summary>
        /// Each player resets the position of their own goal area while in paused and then updates the 
        /// other players raising an event
        /// </summary>
        void ResetGoalAreaPosition()
        {
            // Get a random position
            Vector2 hBound, vBound;
            LevelManager.Instance.GetRandomBoundaries(out hBound, out vBound);
            Vector2 newPosition = new Vector2(Random.Range(hBound.x,hBound.y), Random.Range(vBound.x,vBound.y));

            // Set the local goal area
            UpdateGoalAreaPosition(localGoalArea.transform, newPosition);

            // Raise an event to update the opponent player
            object[] content = new object[] { newPosition }; // Array contains the target position
            RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; 
            PhotonNetwork.RaiseEvent(EventCode.ResetGoalArea, content, raiseEventOptions, SendOptions.SendReliable);
        }

        /// <summary>
        /// Physically adjust the position of the target goal area in the scene.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="newPosition"></param>
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
     
        
        /// <summary>
        /// Call on all the clients when room properties get updated
        /// </summary>
        /// <param name="propertiesThatChanged"></param>
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


        /// <summary>
        /// If the opponent player leaves the room while playing you win 
        /// </summary>
        /// <param name="otherPlayer"></param>
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

        /// <summary>
        /// Handle events
        /// </summary>
        /// <param name="photonEvent"></param>
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

