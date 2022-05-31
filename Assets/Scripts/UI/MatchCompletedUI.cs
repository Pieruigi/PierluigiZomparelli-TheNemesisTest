using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TheNemesis
{
    public class MatchCompletedUI : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        TMP_Text text;

        bool opponentQuit = false;

        // Start is called before the first frame update
        void Start()
        {
            text.gameObject.SetActive(false);

            MatchController.Instance.OnOpponentQuit += () => { opponentQuit = true; ShowYouWin(); };
        }

        // Update is called once per frame
        void Update()
        {

        }

        void Show(string textStr)
        {
            text.text = textStr;
            text.gameObject.SetActive(true);
        }

        public void ShowYouWin()
        {
            Show("You Win");
        }

        public void ShowYouLose()
        {
            Show("You Lose");
        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            // Already completed
            if (opponentQuit)
                return;

            if (propertiesThatChanged.ContainsKey(RoomCustomPropertyUtility.MatchStateKey))
            {
                int matchState = (byte)RoomCustomPropertyUtility.GetMatchState(PhotonNetwork.CurrentRoom);
                if(matchState == (byte)MatchState.Completed)
                {
                    // You team
                    int yourTeam = PlayerCustomPropertyUtility.GetTeamId(PhotonNetwork.LocalPlayer);

                    // Read scores
                    int blueScore = RoomCustomPropertyUtility.GetBlueScore(PhotonNetwork.CurrentRoom);
                    int redScore = RoomCustomPropertyUtility.GetRedScore(PhotonNetwork.CurrentRoom);

                    if((blueScore > redScore && yourTeam == (byte)Team.Blue) || (blueScore < redScore && yourTeam == (byte)Team.Red))
                    {
                        ShowYouWin();
                    }
                    else
                    {
                        ShowYouLose();
                    }
                }
            }

          

        }
    }

}
