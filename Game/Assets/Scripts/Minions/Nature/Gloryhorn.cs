using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloryhorn : Minion, IWarchief
{
    private List<Minion> wounded_minions = null;
    [SerializeField] private int healing_amount = 2;
    public void ActivateWarchiefEffect()
    {
        if (wounded_minions == null)
        {
            wounded_minions = owner.GetMinionsOnBoard(true);
            wounded_minions.Remove(this);
        }

        if (wounded_minions.Count > 0)
        {
            int chosen = Random.Range(0, wounded_minions.Count);
            wounded_minions[chosen].ModifyLife(healing_amount, true);

            if (!wounded_minions[chosen].IsWounded())
            {
                wounded_minions.RemoveAt(chosen);
            }
        }
        else
        {
            owner.Heal(healing_amount);
        }
    }

    public void DeactivateWarchiefEffect()
    {

    }

    public override void OnReceiveDamage(IDamagable source, int physical_damage, int spell_damage)
    {
        base.OnReceiveDamage(source, physical_damage, spell_damage);

        if (life > 0)
        {
            ActivateWarchiefEffect();
        }
    }

    public override void OnNewTurnUpdate()
    {
        base.OnNewTurnUpdate();
        wounded_minions = null;
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // As Gloryhorn always heals
        return true;
    }
}
