using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TheNemesis.UI
{
    public class DelayTimer : MonoBehaviourPunCallbacks
    {
        [SerializeField]
        TMP_Text delayText;

        double startTime;

        // Start is called before the first frame update
        void Start()
        {
            int state = RoomCustomPropertyUtility.GetMatchState(PhotonNetwork.CurrentRoom);
            if (state == (byte)MatchState.Paused)
            {
                startTime = RoomCustomPropertyUtility.GetStartTime(PhotonNetwork.CurrentRoom);
                delayText.gameObject.SetActive(true);
            }
            else
            {
                delayText.gameObject.SetActive(false);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (delayText.gameObject.activeSelf)
            {
                int time = Mathf.Max(0, (int)(startTime - PhotonNetwork.Time));
                delayText.text = time.ToString();
            }
            //int state = RoomCustomPropertyUtility.GetMatchState(PhotonNetwork.CurrentRoom);

            //if(state == (byte)MatchState.Paused)
            //{
            //    delayText.gameObject.SetActive(true);
            //    double startTime = RoomCustomPropertyUtility.GetStartTime(PhotonNetwork.CurrentRoom);
            //    int time = Mathf.Max(0, (int)(startTime - PhotonNetwork.Time));
            //    delayText.text = time.ToString();
            //}
            //else
            //{
            //    delayText.gameObject.SetActive(false);
            //}

        }

        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);

            if (propertiesThatChanged.ContainsKey(RoomCustomPropertyUtility.MatchStateKey))
            {
                int state = RoomCustomPropertyUtility.GetMatchState(PhotonNetwork.CurrentRoom);
                if(state == (byte)MatchState.Paused)
                {
                    startTime = RoomCustomPropertyUtility.GetStartTime(PhotonNetwork.CurrentRoom);
                    delayText.gameObject.SetActive(true);
                }
                else
                {
                    delayText.gameObject.SetActive(false);
                }
            }
          

        }
    }

}
