using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electrocuted : Spell, IActivateWithTarget
{
    [SerializeField] private int additional_damage = 2;
    [SerializeField] private int additional_life = 2;

    public override void Activate(bool using_gold = true)
    {
        if (owner.GetCardsOnBoard().Count > 0)
        {
            base.Activate(using_gold);
        }
        else
        {
            visualizer.OnEndDrag(null);
        }
    }

    public void Activate(bool using_gold = true, GameObject target = null)
    {
        if (owner.GetCardsOnBoard().Count > 0)
        {
            base.Activate(using_gold);

            Card target_card;
            if (target == null)
            {
                Card _ = Heuristic.GetMostValuedMinion(owner, APIHeuristic.Highest_Overall, true).GetComponent<Card>();

                SetLogTargetPosition(owner.GetMinionsOnBoard().IndexOf(_ as Minion) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                target_card = _;
            }
            else
            {
                Card _ = target.GetComponent<Card>();

                SetLogTargetPosition(owner.GetMinionsOnBoard().IndexOf(_ as Minion) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                target_card = _;
            }

            GameManager.GetInstance().NotifyCardIsClicked(target_card, target_card.GetOwner());
        }
    }

    public override void AssignTarget(Card target)
    {
        Minion minion = target as Minion;

        minion.ModifyDamage(additional_damage);
        minion.ModifyLife(additional_life);

        GameManager.GetInstance().NotifyAttackIsOver();

        base.AssignTarget(target);

        VanishCard();
    }
}
