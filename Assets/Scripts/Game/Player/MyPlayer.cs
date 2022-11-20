using Photon.Pun;
using System;
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
    

    public RoleType role;
    public CharacterState CurrentCharacterState { get; private set; }
    public PlayerPropertiesManager properties;

    [Header("Player Objects")]
    public Renderer playerRenderer;

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
        if (properties == null)
        {
            properties = GetComponent<PlayerPropertiesManager>();
        }
    }

    public IEnumerator DeactivateRenderer(float duration)
    {
        playerRenderer.enabled = false;
        yield return new WaitForSeconds(duration);
        playerRenderer.enabled = true;
    }
}
