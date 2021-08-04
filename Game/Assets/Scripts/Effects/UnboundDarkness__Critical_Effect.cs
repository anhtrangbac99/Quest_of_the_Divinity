using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnboundDarkness__Critical_Effect : Effect
{
    [Range(0, 100)][SerializeField] private int critical_chance_modifier = 25;
    public override void TakeEffect()
    {
        if (!activated) return;

        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            List<Card> cards = player.GetCardsOnBoard();
            foreach (Minion minion in cards)
            {
                minion.ModifyCriticalChance(critical_chance_modifier);
            }
        }
    }

    public override void RevertEffect()
    {
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            List<Card> cards = player.GetCardsOnBoard();
            foreach (Minion minion in cards)
            {
                minion.ModifyCriticalChance(-critical_chance_modifier);
            }
        }
        base.RevertEffect();
    }

    public override void TakeEffect(Card card)
    {
        if (!activated) return;
        
        Minion minion = card as Minion;
        if (minion != null)
        {
            minion.ModifyCriticalChance(critical_chance_modifier);
        }
    }
}
