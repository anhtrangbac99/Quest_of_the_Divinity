using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bloodlust : Minion
{
    [SerializeField] private int healing_amount = 1;

    protected override void OnStrikeCallback(IDamagable target, int physical_damage, int spell_damage)
    {
        base.OnStrikeCallback(target, physical_damage, spell_damage);
        ModifyLife(healing_amount, true);
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // This minion always heals when attack.
        return true;
    }
}
