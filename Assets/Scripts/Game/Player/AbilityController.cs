using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;


public class AbilityController : MonoBehaviourPunCallbacks
{
    public MyPlayer owner;
    public int maxAbilities = 3;

    public List<AbilityHolder> abilityHolders = new List<AbilityHolder>();

    private void Start()
    {
        if (owner == null)
        {
            owner = GetComponent<MyPlayer>();
        }
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
}
