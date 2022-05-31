using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheNemesis
{
    public class GameManager : MonoBehaviourPunCallbacks
    {
        public static GameManager Instance { get; private set; }

        bool inGame = false;
        
        bool launchingGame;
        int gameSceneBuildIndex = 1;
        int mainSceneBuildIndex = 0;

      
        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                // Handle scene loading
                SceneManager.sceneLoaded += HandleOnSceneLoaded;

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
            if (inGame) // In game scene
            {
                // Master client only
                if (PhotonNetwork.IsMasterClient)
                {

                }
                

            }
            else // In menu scene
            {
                // Master client only
                if (PhotonNetwork.IsMasterClient)
                {
                    if (!launchingGame)
                    {
                        if (AllPlayersAreReady())
                            StartCoroutine(LaunchGame());
                    }
                }
                
            }
        }

        void HandleOnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if(scene.buildIndex == mainSceneBuildIndex) // Main scene
            {
                inGame = false;
                
                // You can do stuff here while in main menu ( not in game )
            }
            else
            {
                // Game scene has just been loaded, we instantiate over the network the local player on each
                // client and only the master client instantiates and owns the ball
                inGame = true;
                
                if(PhotonNetwork.IsMasterClient)
                    launchingGame = false;

                // Get the spawn point for the local player 
                Transform spawnPoint = LevelManager.Instance.GetSpawnPoint(PhotonNetwork.LocalPlayer);

                // Create the local player
                GameObject player = PhotonNetwork.Instantiate(System.IO.Path.Combine(Constants.PlayerResourceFolder, Constants.DefaultPlayerPrefabName), spawnPoint.position, spawnPoint.rotation);

                // Spawn the ball ( supports host migration )
                if (PhotonNetwork.IsMasterClient)
                {
                    PhotonNetwork.InstantiateRoomObject(System.IO.Path.Combine(Constants.BallResourceFolder, Constants.DefaultBallPrefabName), LevelManager.Instance.GetBallSpawnPoint().position, Quaternion.identity);
                }
            }
            
        }

        bool AllPlayersAreReady()
        {
            if (PhotonNetwork.CurrentRoom == null)
                return false; // No room, no players

            if (PhotonNetwork.CurrentRoom.PlayerCount < PhotonNetwork.CurrentRoom.MaxPlayers)
                return false; // Room not full yet

            foreach(Player player in PhotonNetwork.CurrentRoom.Players.Values)
            {
                // Check if the current player already belongs to a team
                int teamId = PlayerCustomPropertyUtility.GetTeamId(player);
                if (teamId == 0)
                    return false; // TeamId == 0 means no team yet
            }

            return true;
        }

        /// <summary>
        /// Called by the master client only
        /// </summary>
        /// <returns></returns>
        IEnumerator LaunchGame()
        {
            
            // Setting flag
            launchingGame = true;

            // Save room custom properties
            MatchController.SetMatchPaused();
            RoomCustomPropertyUtility.SetBlueScore(PhotonNetwork.CurrentRoom, 0);
            RoomCustomPropertyUtility.SetRedScore(PhotonNetwork.CurrentRoom, 0);
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);

            // Wait for data synchronization
            yield return new WaitForSeconds(1f);

            // Load scene on all clients
            PhotonNetwork.LoadLevel(gameSceneBuildIndex);
        }

        

        #region pun callbacks
        /// <summary>
        /// Remote player entered
        /// </summary>
        /// <param name="newPlayer"></param>
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            base.OnPlayerEnteredRoom(newPlayer);
        }

        /// <summary>
        /// Local player joined room
        /// </summary>
        public override void OnJoinedRoom()
        {
            base.OnJoinedRoom();

        }

        /// <summary>
        /// Local player left room
        /// </summary>
        public override void OnLeftRoom()
        {
            base.OnLeftRoom();

            // Remove player custom properties
            PhotonNetwork.LocalPlayer.CustomProperties.Clear();

            if (inGame)
            {
                PhotonNetwork.LoadLevel(mainSceneBuildIndex);
            }
        }
        #endregion
    }

}
