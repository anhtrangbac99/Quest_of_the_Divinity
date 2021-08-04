using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum ReduceGoldCardType
{
    Minion, Spell, All
}

public class ReduceGoldToActivateCard : Effect
{
    [SerializeField] private int reduce_amount = 1;
    [SerializeField] private int maximum_card_to_reduce = 1;
    [SerializeField] private ReduceGoldCardType reduce_type = ReduceGoldCardType.All;
    public override void TakeEffect()
    {
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            player.placing_card_notification += ReduceGoldToActivateThisCard;
        }
    }

    private void ReduceGoldToActivateThisCard(Player player, Card card)
    {
        if (player == null && card == null)
        {
            if (--maximum_card_to_reduce <= 0)
            {
                RevertEffect();
            }
            return;
        }

        if ((reduce_type == ReduceGoldCardType.Minion && !(card is Minion))
        || (reduce_type == ReduceGoldCardType.Spell && !(card is Spell)))
        {
            return;
        }

        player.ModifyGoldToPlaceCard(reduce_amount);
    }

    public override void RevertEffect()
    {
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            player.placing_card_notification -= ReduceGoldToActivateThisCard;
        }
        base.RevertEffect();
    }
}
