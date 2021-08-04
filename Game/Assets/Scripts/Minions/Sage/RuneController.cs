using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RuneController : Minion, IWarchief
{
    [SerializeField] private int healing_ammount = 1;

    public override void Activate(bool using_gold = true)
    {
        base.Activate(using_gold);
        if (activated)
        {
            GameManager.GetInstance().spell_using_notifier += ActivateWarchiefEffect;
        }
    }

    public void OnDestroy()
    {
        GameManager.GetInstance().spell_using_notifier -= ActivateWarchiefEffect;
    }

    public override void Death()
    {
        base.Death();
        GameManager.GetInstance().spell_using_notifier -= ActivateWarchiefEffect;
    }

    public void ActivateWarchiefEffect()
    {
        owner.Heal(healing_ammount);
    }

    public void DeactivateWarchiefEffect()
    {

    }

    public override void ModifyDamage(int amount, bool is_permanent = false)
    {
        added_spell_damage += amount;
        int final_damage = spell_damage + added_spell_damage;

        visualizer.IndicateBaseDamage(final_damage);
    }

    protected override void ShowDamageAndLife()
    {
        visualizer.IndicateBaseDamage(spell_damage);
        visualizer.IndicateBaseLife(life);
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // As he always heals the player if a spell is casted
        return true;
    }
}
