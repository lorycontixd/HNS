using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Invisibility Ability", menuName = "Abilities/Runner/Invisibility")]
public class InvisibilityAbility : Ability
{
    public override void Initialize(AbilityHolder controller)
    {
        this.holder = controller;
    }

    public override void Fire(AbilityHolder controller)
    {
        MyPlayer player = controller.owner;
        this.holder.StartCoroutine(player.DeactivateRenderer(this.duration));
        PunAbilityEventSender.SendInvisibilityEvent(PhotonNetwork.LocalPlayer, this.duration);
    }
    
}
