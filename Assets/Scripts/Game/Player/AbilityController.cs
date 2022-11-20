using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class AbilityController : MonoBehaviourPunCallbacks
{
    public MyPlayer owner;

    public List<AbilityHolder> abilityHolders = new List<AbilityHolder>();

    private IEnumerator Start()
    {
        if (owner == null)
        {
            owner = GetComponent<MyPlayer>();
        }
        yield return new WaitUntil(() => GameNetworkManager.Instance.gameState == GameState.PREGAME);
    }

    public void OnFire1(InputValue value)
    {
        if (abilityHolders.Count > 0)
        {
            if (abilityHolders[0].ability != null)
            {
                abilityHolders[0].Fire();
            }
        }
    }
    public void OnFire2(InputValue value)
    {
        if (abilityHolders.Count > 1)
        {
            if (abilityHolders[1].ability != null)
            {
                abilityHolders[1].Fire();
            }
        }
    }

    public void AssignAbilities(List<Ability> roleabilities)
    {
        Debug.Log("role abilities coutn : " + roleabilities.Count);
        for (int i=0; i<roleabilities.Count; i++)
        {
            AbilityHolder holder = gameObject.AddComponent<AbilityHolder>();
            holder.Initialize(owner, roleabilities[i]);
            abilityHolders.Add(holder);
        }
    }
}
