using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceDamageTaken : Effect
{
    [SerializeField] private int maximum_damage_to_reduce = 3;
    public override void TakeEffect()
    {
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            if (target_type == EffectTargetType.Player)
            {
                player.player_receive_damage_notifier += ReduceRawDamage;
            }
            else if (target_type == EffectTargetType.Minion)
            {
                player.minion_receive_damage_notifier += ReduceRawDamage;
            }
        }
    }

    private void ReduceRawDamage(int raw_damage)
    {
        if (raw_damage < 0)
        {
            int reduced = (Mathf.Abs(raw_damage) > maximum_damage_to_reduce ? maximum_damage_to_reduce : Mathf.Abs(raw_damage));

            
            attached_target.GetComponent<Player>().Heal(reduced);
        }
    }

    private void ReduceRawDamage(Minion minion, int raw_damage)
    {
        if (raw_damage < 0)
        {
            int reduced = (Mathf.Abs(raw_damage) > maximum_damage_to_reduce ? maximum_damage_to_reduce : Mathf.Abs(raw_damage));

            minion.ModifyLife(reduced, true);
        }
    }

    public override void RevertEffect()
    {
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            if (target_type == EffectTargetType.Player)
            {
                player.player_receive_damage_notifier -= ReduceRawDamage;
            }
            else if (target_type == EffectTargetType.Minion)
            {
                player.minion_receive_damage_notifier -= ReduceRawDamage;
            }
        }
        base.RevertEffect();
    }
}
