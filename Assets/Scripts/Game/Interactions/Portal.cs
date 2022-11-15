using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviourPunCallbacks
{
    public int id = 0;
    public Portal destination;
    public bool isActive;

    public float enterCooldown = 5f;
    private float portalTimestamp;



    private void OnTriggerEnter(Collider other)
    {
        if (isActive)
        {
            if (portalTimestamp <= Time.time)
            {
                if (other.tag == "Player")
                {
                    Player controller = other.gameObject.GetComponent<PhotonView>().Controller;
                    //photonView.RPC("OnEnter", RpcTarget.All, controller);
                    OnEnter(controller, other.gameObject);
                    AddCooldown();
                    destination.AddCooldown();
                }
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        
    }

    [PunRPC]
    public void OnEnterRPC(Player player)
    {

    }
    public void OnEnter(Player player, GameObject playerObj)
    {
        playerObj.GetComponent<CharacterController>().enabled = false;
        playerObj.transform.position = destination.transform.position;
        playerObj.GetComponent<CharacterController>().enabled = true;
    }

    public void AddCooldown()
    {
        portalTimestamp = Time.time + enterCooldown;
    }
}
