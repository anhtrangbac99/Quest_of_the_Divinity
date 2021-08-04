using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForsakenWitch : Minion, IWarchief
{
    [SerializeField] private int curse_damage = 1;
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
        owner.attack_notification += CurseDamage;
    }

    public void DeactivateWarchiefEffect()
    {
        owner.attack_notification -= CurseDamage;
    }

    private void CurseDamage(IDamagable source, IDamagable target)
    {
        if (source != GetComponent<IDamagable>())
        {
            target.OnReceiveDamage(null, 0, curse_damage);
        }
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/

    protected override bool HasAvailableSpecialEffect()
    {
        // As she always have the warchief effect.
        return true;
    }
}
