using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneRitual : Spell, IActivateWithTarget
{
    public override void Activate(bool using_gold = true)
    {
        if (GameManager.GetInstance().GetOpponentsMinions(owner).Count >= 1)
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
        if (GameManager.GetInstance().GetOpponentsMinions(owner).Count > 0)
        {
            base.Activate(using_gold);

            Card target_card;
            if (target == null)
            {
                Card _ = Heuristic.GetMostValuedMinion(owner, APIHeuristic.Lowest_Life).GetComponent<Card>();

                SetLogTargetPosition(GameManager.GetInstance().GetOpponentsMinions(owner).IndexOf(_) + Constants.LOG__TARGET_OPPONENT_CARD_OFFSET);

                target_card = _;
            }
            else
            {
                Card _ = target.GetComponent<Card>();

                SetLogTargetPosition(GameManager.GetInstance().GetOpponentsMinions(owner).IndexOf(_) + Constants.LOG__TARGET_OPPONENT_CARD_OFFSET);

                target_card = _;
            }

            GameManager.GetInstance().NotifyCardIsClicked(target_card, target_card.GetOwner());
        }
    }

    public override void AssignTarget(Card target)
    {
        if (target.GetComponent<Minion>().GetCurrentLife() > spell_damage)
        {
            Deactivate(this);
            GameManager.GetInstance().NotifyAttackIsOver();

        }
        else
        {
            target.GetComponent<IDamagable>().OnReceiveDamage(null, 0, spell_damage);
            base.AssignTarget(target);

            GameManager.GetInstance().NotifyAttackIsOver();
            VanishCard();
        }
    }
}
