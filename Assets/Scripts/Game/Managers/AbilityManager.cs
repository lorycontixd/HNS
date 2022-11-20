using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    private static AbilityManager _instance;
    public static AbilityManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    #endregion

    public int maxAbilities = 3;
    public List<Ability> hunterAbilities = new List<Ability>();
    public List<Ability> runnerAbilities = new List<Ability>();
    public System.Random rnd = new System.Random();


    public List<Ability> GetRoleAbilities(RoleType role)
    {
        if (role == RoleType.HUNTER)
        {
            return hunterAbilities.OrderBy(x => rnd.Next()).Take(maxAbilities).ToList();
        }
        else
        {
            return runnerAbilities.OrderBy(x => rnd.Next()).Take(maxAbilities).ToList();
        }
    }

    public List<Ability> GetRoleAbilities(bool isHunter)
    {
        RoleType role = isHunter ? RoleType.HUNTER : RoleType.RUNNER;
        return GetRoleAbilities(role);
    }

}
