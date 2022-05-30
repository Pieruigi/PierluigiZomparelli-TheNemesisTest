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


        private void Awake()
        {
            if (!Instance)
            {
                Instance = this;

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

        }

        #region public methods
        public Transform GetSpawnPoint(Player player)
        {
            int teamId = PlayerCustomPropertyUtility.GetTeamId(player);
            return spawnPoints[teamId - 1];
        }

        public Transform GetBallSpawnPoint()
        {
            return spawnPoints[2];
        }
        #endregion

    }

}
