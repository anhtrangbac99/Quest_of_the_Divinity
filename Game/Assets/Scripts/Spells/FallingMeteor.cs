using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingMeteor : Spell
{
    public override void Activate(bool using_gold = true)
    {
        if (!owner.PlaceThisCard(this, false, using_gold))
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
            List<Card> enemies = GameManager.GetInstance().GetOpponentsMinions(owner);
            List<Card> allies = owner.GetCardsOnBoard();

            DealSpellDamage(enemies);
            DealSpellDamage(allies);

            if (GameManager.GetInstance().LOG)
            {
                GameManager.GetInstance().LogPlayCard(log__card_position);
            }

            VanishCard();
        }
    }

    private void DealSpellDamage(List<Card> cards)
    {
        foreach(IDamagable i in cards)
        {
            if (i != null)
            {
                i.OnReceiveDamage(null, 0, spell_damage);
            }
        }
    }
}
