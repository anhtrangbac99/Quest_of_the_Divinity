using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class The_Enlightened__Add_Life_Effect : Effect
{
    [SerializeField] private int additional_life = 1;
    public override void TakeEffect()
    {
        if (!activated) return;
        
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            List<Card> cards = player.GetCardsOnBoard();
            foreach(Minion _ in cards)
            {
                if (!_.CompareTag("No_Add_Life"))
                {
                    _.ModifyLife(additional_life);
                }
            }
        }
    }

    public override void TakeEffect(Card card)
    {
        if (activated)
        {
            Minion _ = card as Minion;
            if (!_.CompareTag("No_Add_Life"))
            {
                _.ModifyLife(additional_life);
            }
        }
    }
}
