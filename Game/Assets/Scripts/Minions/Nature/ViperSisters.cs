using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViperSisters : Minion
{
    [SerializeField] private Effect reduce_damage_effect = null;

    public override void Strike(bool duel = false)
    {
        Minion _ = target as Minion;
        if (_ != null)
        {
            _.AddEffect(reduce_damage_effect);
        }

        base.Strike(duel);
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // This minion always reduce damage.
        return true;
    }
}
