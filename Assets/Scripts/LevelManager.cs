using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    /// <summary>
    /// You can ask this class to gat player and ball spawn points, goal area spawn area ecc.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        /// <summary>
        /// Ids:
        /// 0 - blue player
        /// 1 - red player
        /// 2 - ball
        /// </summary>
        [SerializeField]
        List<Transform> spawnPoints; 

        /// <summary>
        /// Goal area volumes for random spawning
        /// 0 - in the blue player midfield
        /// 1 - in the red player midfield
        /// </summary>
        [SerializeField]
        List<BoxCollider> volumes; // To get goal area random spawn points

        /// <summary>
        /// Horizontal and vertical boundaries ( x and Z ) for goal area random positions; we fill these
        /// using above volumes
        /// 0 - blue area
        /// 1 - red area
        /// </summary>
        List<Vector2> horizontalBoundaries = new List<Vector2>();
        List<Vector2> verticalBoundaries = new List<Vector2>();

        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

                
                // Get boundaries using volumes min and max bounds
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
        /// Returns the spawn point of a specific player ( blue or red )
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
        /// Returns horizontal boundaries to randomize a specific goal area position ( blue or red )
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public Vector2 GetHorizontalGoalAreaBoundaries(int team)
        {
            return horizontalBoundaries[team - 1];
        }

        /// <summary>
        /// Returns vertical boundaries to randomize a specific goal area position ( blue or red )
        /// </summary>
        /// <param name="team"></param>
        /// <returns></returns>
        public Vector2 GetVerticalGoalAreaBoundaries(int team)
        {
            return verticalBoundaries[team - 1];
        }

        /// <summary>
        /// Returns random boundaries that are independent by the color of the area; so you can spawn 
        /// blue goal area both in the blue and red player midfield
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        public void GetRandomBoundaries(out Vector2 horizontal, out Vector2 vertical)
        {
            int i = Random.Range(0, 2);
            horizontal = horizontalBoundaries[i];
            vertical = verticalBoundaries[i];
        }
        #endregion

    }

}
