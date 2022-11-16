using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviourPunCallbacks
{
    public bool isActive;
    public bool isOpen;

    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (GameNetworkManager.Instance.gameState == GameState.WAITINGFORPLAYERS)
        {
            if (isOpen)
            {
                Close();
            }
        }
    }

    public void Open()
    {
        isOpen = true;
        animator.SetBool("IsOpen", true);
    }
    public void Close()
    {
        isOpen = false;
        animator.SetBool("IsOpen", false);
    }
}
