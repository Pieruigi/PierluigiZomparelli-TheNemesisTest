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
            if (MatchController.Instance.State == (byte)MatchState.Completed)
                return;

            if (Tags.Ball.Equals(other.tag))
            {
                int score = 0;
                if (team == Team.Blue)
                    score = RoomCustomPropertyUtility.GetRedScore(PhotonNetwork.CurrentRoom) + 1;
                else
                    score = RoomCustomPropertyUtility.GetBlueScore(PhotonNetwork.CurrentRoom) + 1;
              
                if (PhotonNetwork.IsMasterClient)
                {
                    if (score == Constants.GoalsToWin)
                        MatchController.SetMatchCompleted();
                        //RoomCustomPropertyUtility.SetMatchState(PhotonNetwork.CurrentRoom, (byte)MatchState.Completed);

                    if (team == Team.Blue)
                        RoomCustomPropertyUtility.SetRedScore(PhotonNetwork.CurrentRoom, (byte)score);
                    else
                        RoomCustomPropertyUtility.SetBlueScore(PhotonNetwork.CurrentRoom, (byte)score);

                    // Synch properties
                    PhotonNetwork.CurrentRoom.SetCustomProperties(PhotonNetwork.CurrentRoom.CustomProperties);

                    // Change state
                    if (score < Constants.GoalsToWin)
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

            //RoomCustomPropertyUtility.SetMatchState(PhotonNetwork.CurrentRoom, (byte)MatchState.Paused);
            //RoomCustomPropertyUtility.SetStartTime(PhotonNetwork.CurrentRoom, PhotonNetwork.Time + Constants.StartDelay);
            MatchController.SetMatchPaused();
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

        public bool IsLocalGoalArea()
        {
            // Get local player team
            int localTeam = (byte)PlayerCustomPropertyUtility.GetTeamId(PhotonNetwork.LocalPlayer);
            if (localTeam == (byte)team)
                return true;
            else
                return false;
        }

        public Vector2 GetHorizontalBoundaries()
        {
            return LevelManager.Instance.GetHorizontalGoalAreaBoundaries((byte)team);
        }

        public Vector2 GetVerticalBoundaries()
        {
            return LevelManager.Instance.GetVerticalGoalAreaBoundaries((byte)team);
        }
    }

}
