using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GainDamageAndLife : Effect
{
    [SerializeField] private int additional_damage = 1;
    [SerializeField] private int additional_life = 2;
    public override void TakeEffect()
    {
        if (!activated) return;

        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            List<Card> on_board_minions = player.GetCardsOnBoard();
            foreach(Minion _ in on_board_minions)
            {
                if (_.GetClass() == MinionClass.Dragon && !(_ is TheOneAboveAll))
                {
                    _.ModifyDamage(additional_damage);
                    _.ModifyLife(additional_life);
                }
            }
        }
    }

    public override void TakeEffect(Card card)
    {
        if (!activated) return;

        Minion _ = card as Minion;
        if (_ != null && _.GetClass() == MinionClass.Dragon && !(_ is TheOneAboveAll))
        {
            _.ModifyDamage(additional_damage);
            _.ModifyLife(additional_life);
        }
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            List<Card> on_board_minions = player.GetCardsOnBoard();
            foreach(Minion _ in on_board_minions)
            {
                if (_.GetClass() == MinionClass.Dragon && !(_ is TheOneAboveAll))
                {
                    _.ModifyDamage(-additional_damage);
                    _.ModifyLife(-additional_life);
                }
            }
        }
    }
}
