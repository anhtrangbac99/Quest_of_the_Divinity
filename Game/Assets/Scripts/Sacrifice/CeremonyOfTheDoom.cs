using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CeremonyOfTheDoom : Sacrifice
{
    private int num_turns_deducted = 0;
    public override void AssignTarget(Card card)
    {
        num_turns_deducted = card.GetComponent<Minion>().GetCost() / 2;

        base.AssignTarget(card);
    }
    protected override void TakeEffect()
    {
        GameManager.GetInstance().GetOpponent(owner).AddEffectForPlayer(effect_reference, num_turns_deducted);
        base.TakeEffect();
    }
}
