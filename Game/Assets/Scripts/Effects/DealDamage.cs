using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DealDamage : Effect
{
    [SerializeField] private int minimum_spell_damage = 1;
    [SerializeField] private int maximum_spell_damage = 3;
    [SerializeField] private bool random = false;
    public override void TakeEffect()
    {
        Strike();
    }

    private void Strike()
    {
        int final_spell_damage = maximum_spell_damage;
        if (random)
        {
            final_spell_damage = Random.Range(minimum_spell_damage, maximum_spell_damage + 1);
        }

        switch(target_type)
        {
            case EffectTargetType.Minion:
                List<Card> enemy_minions = GameManager.GetInstance().GetOpponentsMinions(attached_target.GetComponent<Player>());
                foreach(IDamagable _ in enemy_minions)
                {
                    _.OnReceiveDamage(null, 0, final_spell_damage);
                }
                break;
            case EffectTargetType.Player:
                IDamagable player = GameManager.GetInstance().GetOpponent(attached_target.GetComponent<Player>()) as IDamagable;
                player.OnReceiveDamage(null, 0, final_spell_damage);
                break;
        }
    }

    public override void OnNewTurnUpdate()
    {
        base.OnNewTurnUpdate();
        if (activated)
        {
            Strike();
        }
    }
}
