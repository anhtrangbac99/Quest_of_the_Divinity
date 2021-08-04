using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UprisingWarmaster : Minion
{
    [SerializeField] private int turns_to_double_attack = 2;
    // [SerializeField] private int double_damage = 0;

    public override void Strike(bool duel = false)
    {
        if (turns_to_double_attack <= 0)
        {
            physical_damage *= 2;
        }

        base.Strike(duel);

        if (turns_to_double_attack <= 0)
        {
            turns_to_double_attack = 2;
        }

        turns_to_double_attack--;
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // Return true on double damage turns.
        if (turns_to_double_attack <= 0)
        {
            return true;
        }

        return base.HasAvailableSpecialEffect();
    }
}
