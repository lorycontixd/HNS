using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbilityHolder : MonoBehaviour
{
    public MyPlayer owner;

    // The ability to trigger
    public Ability ability;

    // The ability state
    public AbilityState currentAbilityState = AbilityState.READY;

    private IEnumerator _handleAbilityUsage;

    public UnityEvent onAbilityTrigger;


    public void Fire()
    {
        if (currentAbilityState != AbilityState.READY)
        {
            return;
        }
        if (!IsCharacterInAllowedState())
        {
            return;
        }

        StartCoroutine(HandleAbilityUsageCoroutine());
    }

    public bool IsCharacterInAllowedState()
    {
        return ability.validCharacterStates.Contains(owner.CurrentCharacterState);
    }

    private IEnumerator HandleAbilityUsageCoroutine()
    {
        currentAbilityState = AbilityState.CASTING;
        yield return new WaitForSeconds(ability.castingTime);
        ability.Fire(this);
        currentAbilityState = AbilityState.COOLDOWN;
        onAbilityTrigger?.Invoke();

        if (ability.cooldown > 0f)
        {
            StartCoroutine(HandleCooldownCoroutine());
        }
    }

    private IEnumerator HandleCooldownCoroutine()
    {
        yield return new WaitForSeconds(ability.cooldown);
        currentAbilityState = AbilityState.READY;
    }
}
