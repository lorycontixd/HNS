using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CharacterState
{
    WALKING,
    RUNNING,
    JUMPING,
    DEAD
}

public class MyPlayer : MonoBehaviourPunCallbacks
{
    [Header("Camera")]
    public GameObject cameraFollow;
    public GameObject camera;

    public CharacterState CurrentCharacterState { get; private set; }
    

    private void Awake()
    {
        if (!photonView.IsMine)
        {
            Destroy(cameraFollow);
            Destroy(camera);
        }
    }
}
