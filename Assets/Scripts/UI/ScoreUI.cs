using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TheNemesis.UI
{
    public class ScoreUI : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        TMP_Text blueText;

        [SerializeField]
        TMP_Text redText;

        // Start is called before the first frame update
        void Start()
        {
            int score = RoomCustomPropertyUtility.GetBlueScore(PhotonNetwork.CurrentRoom);
            blueText.text = score.ToString();
            score = RoomCustomPropertyUtility.GetRedScore(PhotonNetwork.CurrentRoom);
            redText.text = score.ToString();
        }

     

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            if (propertiesThatChanged.ContainsKey(RoomCustomPropertyUtility.BlueScoreKey))
            {
                int score = RoomCustomPropertyUtility.GetBlueScore(PhotonNetwork.CurrentRoom);
                blueText.text = score.ToString();
            }
            if (propertiesThatChanged.ContainsKey(RoomCustomPropertyUtility.RedScoreKey))
            {
                int score = RoomCustomPropertyUtility.GetRedScore(PhotonNetwork.CurrentRoom);
                redText.text = score.ToString();
            }

        }
    }

}
