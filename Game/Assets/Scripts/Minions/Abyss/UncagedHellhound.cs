using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UncagedHellhound : Minion
{
    [SerializeField] private Card hound = null;

    public override void Activate(bool using_gold = true)
    {
        base.Activate(using_gold);
        if (is_guard)
        {
            RemoveGuardAttribute();
        }
        visualizer.GetDamageIndicator().Break();
    }

    protected override void OnStrikeCallback(IDamagable target, int physical_damage, int spell_damage)
    {
        is_first_turn = false;

        if (life > 0)
        {
            Minion _ = target as Minion;


            if (_ != null && _.IsDead() && owner.CanPlaceCard())
            {
                visualizer.StopAttack();

                hound.GetComponent<MinionVisualizer>().ChangeCardAppearance(false);

                Minion h = Instantiate(hound, Vector3.zero, transform.rotation) as Minion;

                h.SetOwner(owner);
                h.Activate(false);

                hound.GetComponent<MinionVisualizer>().ChangeCardAppearance(true, false);
            }

        }
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // This brat summons on kill
        return true;
    }
}
