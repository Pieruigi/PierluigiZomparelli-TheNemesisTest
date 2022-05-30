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
                if (PhotonNetwork.IsMasterClient)
                {
                    int score = 0;
                    // Update score
                    if (team == Team.Blue)
                    {
                        score = RoomCustomPropertyUtility.GetRedScore(PhotonNetwork.CurrentRoom) + 1;
                        RoomCustomPropertyUtility.SetRedScore(PhotonNetwork.CurrentRoom, (byte)score);
                    }
                    else
                    {
                        score = RoomCustomPropertyUtility.GetBlueScore(PhotonNetwork.CurrentRoom) + 1;
                        RoomCustomPropertyUtility.SetBlueScore(PhotonNetwork.CurrentRoom, (byte)score);
                    }

                    // Change state to paused
                    StartCoroutine(SetMatchPaused());
                }

                // All clients hide the ball
                other.GetComponent<BallController>().Explode();

                StartCoroutine(ResetBall());
                StartCoroutine(ResetLocalPlayer());
            }
        }

        IEnumerator SetMatchPaused()
        {
            if (!PhotonNetwork.IsMasterClient)
                yield break;

            yield return new WaitForSeconds(delayOnGoal);
            
            RoomCustomPropertyUtility.SetMatchState(PhotonNetwork.CurrentRoom, (byte)MatchState.Paused);
            RoomCustomPropertyUtility.SetStartTime(PhotonNetwork.CurrentRoom, PhotonNetwork.Time);
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
