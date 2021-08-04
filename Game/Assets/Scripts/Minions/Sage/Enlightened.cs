using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enlightened : Minion, IWarchief
{
    [SerializeField] private Effect the_enlightened__add_life_effect = null;

    public override void Activate(bool using_gold = true)
    {
        base.Activate(using_gold);
        if (activated)
        {
            ActivateWarchiefEffect();
        }
    }

    public override void Death()
    {
        DeactivateWarchiefEffect();
        base.Death();
    }

    public void ActivateWarchiefEffect()
    {
        owner.AddEffectForMinion(the_enlightened__add_life_effect);
    }

    public void DeactivateWarchiefEffect()
    {
        owner.RemoveEffectFromMinion(the_enlightened__add_life_effect);
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // As The Enlighten always grants allies life.
        return true;
    }
}
