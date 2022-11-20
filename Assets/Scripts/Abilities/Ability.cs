using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Realtime;
using System;

public enum AbilityType
{
    BOOST,
    SCAN,
    HIDE
}

public enum AbilityState
{
    READY = 0,
    CASTING = 1,
    COOLDOWN = 2
}

[Serializable]
public abstract class Ability : ScriptableObject
{
    public string abilityName;
    public string description;
    public RoleType role;
    public AbilityType type;
    public List<CharacterState> validCharacterStates = new List<CharacterState>() { CharacterState.WALKING, CharacterState.RUNNING, CharacterState.JUMPING };
    public List<GameState> validGameStates = new List<GameState>() { GameState.SEARCHPHASE };
    public List<string> tags = new List<string>();

    public float duration;
    public float manaCost;
    public float castingTime;
    public float cooldown;

    //States
    public bool onCooldown;

    public UnityEvent<Ability> onFired;
    public UnityEvent<Ability> onReady;

    protected AbilityHolder holder;
    protected float cooldownTimestamp;

    public virtual void OnAbilityUpdate(AbilityHolder controller) { }
    public abstract void Initialize(AbilityHolder controller);
    public abstract void Fire(AbilityHolder controller);

    public void Tick()
    {
        cooldownTimestamp -= Time.time;
    }
}
