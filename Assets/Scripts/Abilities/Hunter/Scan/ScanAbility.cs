using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Scan Ability", menuName = "Abilities/Hunter/Scan")]
public class ScanAbility : Ability
{
    public float scanRange;
    private ScanAbilityUI uiManager;
    public GameObject scanArrowUI;

    private List<Player> foundPlayers = new List<Player>();
    private List<GameObject> foundPlayerObjs = new List<GameObject>();
    private GameObject closestPlayerObj = null;
    private Player closestPlayer = null;
    public bool fired;

    public override void Initialize(AbilityHolder holder)
    {
        this.holder = holder;
    }

    public override void Fire(AbilityHolder holder)
    {
        Debug.Log("[ScanAbility] Starting Fire");
        fired = false;
        foundPlayers.Clear();
        foundPlayerObjs.Clear();
        closestPlayerObj = null;
        int maxColliders = 10000;
        Collider[] hitColliders = new Collider[maxColliders];
        int collidersFound = Physics.OverlapSphereNonAlloc(holder.owner.transform.position, scanRange, hitColliders);
        for (int i=0; i<collidersFound; i++)
        {
            if (hitColliders[i].tag == "Player" && hitColliders[i].transform != holder.owner.transform)
            {
                foundPlayerObjs.Add(hitColliders[i].gameObject);
            }
        }
        Debug.Log("Found player objects nearby: " + foundPlayerObjs.Count);
        if (foundPlayerObjs.Count > 0)
        {
            Debug.Log("First obj: " + foundPlayerObjs[0]);
        }

        if (foundPlayerObjs.Count <= 0)
        {
            Debug.Log($"[ScanAbility] No players found on fire, returning");
            return;
        }
        else
        {
            for (int i=0; i<foundPlayerObjs.Count; i++)
            {
                if (closestPlayerObj == null)
                {
                    closestPlayerObj = foundPlayerObjs[i];
                }
                else
                {
                    if (Vector3.Distance(foundPlayerObjs[i].transform.position, holder.owner.transform.position) < Vector3.Distance(closestPlayerObj.transform.position, holder.owner.transform.position))
                    {
                        closestPlayerObj = foundPlayerObjs[i];
                    }
                }
            }
            Debug.Log("Closest player: " + closestPlayerObj);
            closestPlayer = closestPlayerObj.GetComponent<PhotonView>().Controller;
            Debug.Log($"[ScanAbility] Found closest player: {closestPlayer.UserId}   ({closestPlayer.NickName}");
        }

        uiManager = holder.gameObject.AddComponent<ScanAbilityUI>();
        uiManager.Initialize(this, holder.transform, closestPlayerObj.transform, this.duration);
        uiManager.Display();

        fired = true;
        onFired?.Invoke(this);
    }


    public Player GetClosestPlayer()
    {
        return closestPlayer;
    }
    public GameObject GetClosestPlayerObj()
    {
        return closestPlayerObj;
    }
}
