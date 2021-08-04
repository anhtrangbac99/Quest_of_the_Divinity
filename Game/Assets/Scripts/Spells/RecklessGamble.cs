using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecklessGamble : Spell
{
    [SerializeField] private int num_cards = 2;

    public override void Activate(bool using_gold = true)
    {
        if (!owner.PlaceThisCard(this, false, using_gold))
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
            owner.RetrieveCard(num_cards);

            if (GameManager.GetInstance().LOG)
            {
                GameManager.GetInstance().LogPlayCard(log__card_position);
            }
            VanishCard();
        }
    }
}
