using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This effect will always attached to player, but it affects their minions.
public class UnboundDarkness : Sacrifice
{
    protected override void TakeEffect()
    {
        owner.AddEffectForMinion(effect_reference);
        base.TakeEffect();
    }
}
