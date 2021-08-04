using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionDamageAndLifeModifier : Effect
{
    [SerializeField] private int damage_modifier = 0;
    [SerializeField] private int life_modifier = 0;
    public override void TakeEffect()
    {
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            List<Minion> minions = player.GetMinionsOnBoard();
            foreach (Minion _ in minions)
            {
                _.ModifyDamage(damage_modifier);
                _.ModifyLife(life_modifier);
            }
        }
    }

    public override void TakeEffect(Card card)
    {
        Minion _ = card as Minion;
        _.ModifyDamage(damage_modifier);
        _.ModifyLife(life_modifier);
    }

    public override void RevertEffect()
    {
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            List<Minion> minions = player.GetMinionsOnBoard();
            foreach (Minion _ in minions)
            {
                _.ModifyDamage(-damage_modifier);
                _.ModifyLife(-life_modifier);
            }
        }
        base.RevertEffect();
    }
}
