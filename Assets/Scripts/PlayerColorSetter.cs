using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TheNemesis
{
    public class PlayerColorSetter : MonoBehaviour
    {
        [SerializeField]
        List<Material> teamMaterials;

        [SerializeField]
        int materialId;

        // Start is called before the first frame update
        void Start()
        {
            // Get the photon player attached to this photon view
            Player player = transform.root.GetComponent<PhotonView>().Owner;
            // Get the team color
            int teamId = PlayerCustomPropertyUtility.GetTeamId(player);
            
            if(teamId > 0)
            {
                // Get renderer and change material
                MeshRenderer renderer = GetComponent<MeshRenderer>();
                Material[] mats = renderer.materials;
                mats[materialId] = teamMaterials[teamId - 1];
                renderer.materials = mats;
            }
            
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}
