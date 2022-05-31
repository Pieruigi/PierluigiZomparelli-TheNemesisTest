using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [SerializeField]
        List<Transform> spawnPoints;

        [SerializeField]
        List<BoxCollider> volumes; // To get goal area random spawn points

        List<Vector2> horizontalBoundaries = new List<Vector2>();
        List<Vector2> verticalBoundaries = new List<Vector2>();

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                
                // Get boundaries
                for(int i=0; i<2; i++)
                {
                    BoxCollider c = volumes[i];

                    Vector2 h = new Vector2(c.bounds.min.x, c.bounds.max.x) + c.center.x * Vector2.one;
                    Vector2 v = new Vector2(c.bounds.min.z, c.bounds.max.z) + c.center.z * Vector2.one;

                    horizontalBoundaries.Add(h);
                    verticalBoundaries.Add(v);

                    Destroy(c.gameObject); // We can destroy the volume after using it
                }

                // Clear the volume list from null references
                volumes.Clear(); 

            }
            else
            {
                Destroy(gameObject);
            }
        }

       
        #region public methods
        /// <summary>
        /// Returns the spawn point of the specific player
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public Transform GetSpawnPoint(Player player)
        {
            int teamId = PlayerCustomPropertyUtility.GetTeamId(player);
            return spawnPoints[teamId - 1];
        }

        /// <summary>
        /// Returns the ball spawn point
        /// </summary>
        /// <returns></returns>
        public Transform GetBallSpawnPoint()
        {
            return spawnPoints[2];
        }

        /// <summary>
        /// Returns horizontal boundaries to randomize a specific goal area position
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public Vector2 GetHorizontalGoalAreaBoundaries(int team)
        {
            return horizontalBoundaries[team - 1];
        }

        /// <summary>
        /// Returns vertical boundaries to randomize a specific goal area position
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public Vector2 GetVerticalGoalAreaBoundaries(int team)
        {
            return verticalBoundaries[team - 1];
        }

        public void GetRandomBoundaries(out Vector2 horizontal, out Vector2 vertical)
        {
            int i = Random.Range(0, 2);
            horizontal = horizontalBoundaries[i];
            vertical = verticalBoundaries[i];
        }
        #endregion

    }

}
