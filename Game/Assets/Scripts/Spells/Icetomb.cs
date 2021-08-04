using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Icetomb : Spell, IActivateWithTarget
{
    [SerializeField] private Effect freeze_effect = null;

    public override void Activate(bool using_gold = true)
    {
        if (GameManager.GetInstance().GetOpponentsMinions(owner).Count < 1)
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
            base.Activate(using_gold);
        }
    }

    public void Activate(bool using_gold = true, GameObject target = null)
    {
        if (GameManager.GetInstance().GetOpponentsMinions(owner).Count > 0)
        {
            base.Activate(using_gold);

            Card target_card;
            if (target == null)
            {
                Card _ = Heuristic.GetMostValuedMinion(owner, APIHeuristic.Highest_Overall).GetComponent<Card>();

                SetLogTargetPosition(GameManager.GetInstance().GetOpponentsMinions(owner).IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                target_card = _;
            }
            else
            {
                Card _ = target.GetComponent<Card>();

                SetLogTargetPosition(GameManager.GetInstance().GetOpponentsMinions(owner).IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                target_card = _;
            }

            GameManager.GetInstance().NotifyCardIsClicked(target_card, target_card.GetOwner());
        }
    }

    public override void AssignTarget(Card target)
    {
        Minion minion = target as Minion;
        minion.AddEffect(freeze_effect);

        GameManager.GetInstance().NotifyAttackIsOver();

        base.AssignTarget(target);

        VanishCard();
    }
}
