using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPlayer : MonoBehaviourPunCallbacks
{
    [Header("Camera")]
    public GameObject cameraFollow;
    public GameObject camera;
    
    private void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(cameraFollow);
            Destroy(camera);
        }
    }
}
