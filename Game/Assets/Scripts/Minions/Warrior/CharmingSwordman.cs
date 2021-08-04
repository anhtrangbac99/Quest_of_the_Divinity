using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharmingSwordman : Minion, IChampion
{
    [SerializeField] private Effect first_turn_reduce_damage_effect = null;


    public override void Activate(bool using_gold = true)
    {
        base.Activate();
        if (activated)
        {
            ActivateChampionEffect();
        }
    }
    
    public void ActivateChampionEffect()
    {
        List<Card> opponents_cards = GameManager.GetInstance().GetOpponentsMinions(owner);

        foreach(Card opponents_card in opponents_cards)
        {
            opponents_card.GetComponent<Minion>().AddEffect(first_turn_reduce_damage_effect);
        }
    }

    public void DeactivateChampionEffect()
    {   }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        if (IsFirstTurn()) return true;

        return base.HasAvailableSpecialEffect();
    }
}
