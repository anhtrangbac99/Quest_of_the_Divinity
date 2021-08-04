using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiftOfTheNature : Spell
{
    [SerializeField] private int healing_amount = 5;
    
    public override void Activate(bool using_gold = true)
    {
        if (!owner.PlaceThisCard(this, false, using_gold))
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
           owner.Heal(healing_amount);

            if (GameManager.GetInstance().LOG)
            {
                GameManager.GetInstance().LogPlayCard(log__card_position);
            }

           VanishCard(); 
        }
    }
}
