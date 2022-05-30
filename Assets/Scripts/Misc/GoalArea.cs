using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class GoalArea : MonoBehaviour
    {
        [SerializeField]
        Team team;

        float delayOnGoal = 3;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (Tags.Ball.Equals(other.tag))
            {
                int score = 0;
                if (team == Team.Blue)
                    score = RoomCustomPropertyUtility.GetRedScore(PhotonNetwork.CurrentRoom) + 1;
                else
                    score = RoomCustomPropertyUtility.GetBlueScore(PhotonNetwork.CurrentRoom) + 1;
              
                if (PhotonNetwork.IsMasterClient)
                {
                    if(team == Team.Blue)
                        RoomCustomPropertyUtility.SetRedScore(PhotonNetwork.CurrentRoom, (byte)score);
                    else
                        RoomCustomPropertyUtility.SetBlueScore(PhotonNetwork.CurrentRoom, (byte)score);

                    // Synch the score to show it asap
                    PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);

                    // Change state
                    if (score == 3)
                        StartCoroutine(SetMatchCompleted());
                    else
                        StartCoroutine(SetMatchPaused());
                    
                }

                // All clients hide the ball
                other.GetComponent<BallController>().Explode();

                if (score < 3)
                {
                    StartCoroutine(ResetBall());
                    StartCoroutine(ResetLocalPlayer());
                }
                
            }
        }

        IEnumerator SetMatchPaused()
        {
            if (!PhotonNetwork.IsMasterClient)
                yield break;

            yield return new WaitForSeconds(delayOnGoal);
            
            RoomCustomPropertyUtility.SetMatchState(PhotonNetwork.CurrentRoom, (byte)MatchState.Paused);
            RoomCustomPropertyUtility.SetStartTime(PhotonNetwork.CurrentRoom, PhotonNetwork.Time + Constants.StartDelay);
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }

        IEnumerator SetMatchCompleted()
        {
            if (!PhotonNetwork.IsMasterClient)
                yield break;

            yield return new WaitForSeconds(0.5f);

            RoomCustomPropertyUtility.SetMatchState(PhotonNetwork.CurrentRoom, (byte)MatchState.Completed);
            PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);
        }

        IEnumerator ResetBall()
        {
            yield return new WaitForSeconds(delayOnGoal);
            BallController.Instance.Reset();
        }

        IEnumerator ResetLocalPlayer()
        {
            yield return new WaitForSeconds(delayOnGoal);
            PlayerController.LocalPlayerController.Reset();
        }
    }

}
