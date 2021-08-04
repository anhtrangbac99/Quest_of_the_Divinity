using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LionroarGeneral : Minion, IChampion
{
    [SerializeField] private GameObject shield;
    private GameObject real_life_shield = null;

    private bool immune_to_physical = false;

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }

    public override void OnReceiveDamage(IDamagable source, int physical_damage, int spell_damage)
    {
        if (immune_to_physical)
        {
            base.OnReceiveDamage(source, 0, spell_damage);
        }
        else
        {
            base.OnReceiveDamage(source, physical_damage, spell_damage);
        }
    }
    
    public void ActivateChampionEffect()
    {
        immune_to_physical = true;

        if (!GameManager.GetInstance().NON_GRAPHICAL)
        {
            real_life_shield = Instantiate(shield, transform.position, transform.rotation, transform);
        }
    }

    public void DeactivateChampionEffect()
    {
        immune_to_physical = false;

        if (real_life_shield != null)
        {
            real_life_shield.GetComponent<Animator>().SetTrigger("Out");
            Destroy(real_life_shield, 1.5f);
        }
    }

    public override void OnNewTurnUpdate()
    {
        if (is_first_turn)
        {
            is_first_turn = false;
            DeactivateChampionEffect();
        }
        base.OnNewTurnUpdate();
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // Immune to physical damage on first turn
        if (IsFirstTurn()) return true;

        return base.HasAvailableSpecialEffect();
    }
}
