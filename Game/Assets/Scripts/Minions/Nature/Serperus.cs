using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serperus : Minion
{
    [SerializeField] private float adjacent_target_damage_multiplier = 0.5f;

    public override void Strike(bool duel = false)
    {
        Minion _ = target as Minion;
        if (_ != null)
        {           
            List<Card> enemy_minions = GameManager.GetInstance().GetOpponentsMinions(owner);

            if (enemy_minions.Count > 1)
            {
                int final_physical_damage = (int)((physical_damage + added_physical_damage) * adjacent_target_damage_multiplier);

                int final_spell_damage = (int)((spell_damage + added_spell_damage) * adjacent_target_damage_multiplier);

                int base_target_index = enemy_minions.IndexOf(_);

                if (base_target_index - 1 >= 0 && base_target_index != 3)
                {
                    Card chosen_one = enemy_minions[base_target_index - 1];
                    chosen_one.GetComponent<IDamagable>().OnReceiveDamage(null, final_physical_damage, final_spell_damage);

                    chosen_one.GetComponent<Minion>().ScatterDebris(final_physical_damage + final_spell_damage, custom_attack_effect);
                }

                if (base_target_index + 1 < enemy_minions.Count && base_target_index != 2)
                {
                    Card chosen_one = enemy_minions[base_target_index + 1];
                    chosen_one.GetComponent<IDamagable>().OnReceiveDamage(null, final_physical_damage, final_spell_damage);

                    chosen_one.GetComponent<Minion>().ScatterDebris(final_physical_damage + final_spell_damage, custom_attack_effect);
                }
            }
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
        // Deals adjacent damage
        return true;
    }
}
