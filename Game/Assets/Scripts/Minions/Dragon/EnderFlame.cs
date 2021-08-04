using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnderFlame : Dragon
{
    private int last_life_checker;

    public override void Activate(bool using_gold = true)
    {
        base.Activate(using_gold);
        last_life_checker = base_maximum_life;
    }

    public override void OnReceiveDamage(IDamagable source, int physical_damage, int spell_damage)
    {
        base.OnReceiveDamage(source, physical_damage, spell_damage);

        if (activated)
        {
            ModifyDamage(last_life_checker - life);
            last_life_checker = life;          
        }
    }

    public override void ModifyLife(int amount, bool life_only = false)
    {
        base.ModifyLife(amount, life_only);

        if (activated)
        {
            ModifyDamage(last_life_checker - life);
            last_life_checker = life;
        }        
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/

    protected override bool HasAvailableSpecialEffect()
    {
        if (life == base_maximum_life) 
            return false;

        else 
            return true;
    }
}
