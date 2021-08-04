using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpaningSpell : Spell, IActivateWithTarget
{   
    [SerializeField] private int min_minion_to_spawn = 1;
    [SerializeField] private SpellObject obj = null;

    public override void Activate(bool using_gold = true)
    {
        if (GameManager.GetInstance().GetOpponentsMinions(owner).Count < min_minion_to_spawn
            || !owner.PlaceThisCard(this, false, using_gold))
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
            if (obj.NeedsTarget())
            {
                GameManager.GetInstance().NotifySpellRequireTarget(this, owner);
            }
            else
            {
                AssignTarget(null as Card);
            }
        }
    }
    public void Activate(bool using_gold = true, GameObject target = null)
    {
        if (GameManager.GetInstance().GetOpponentsMinions(owner).Count < min_minion_to_spawn
            || !owner.PlaceThisCard(this, false, using_gold))
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
            if (obj.NeedsTarget())
            {
                if (target != null)
                {
                    Card _ = target.GetComponent<Card>();

                    SetLogTargetPosition(GameManager.GetInstance().GetOpponentsMinions(owner).IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                    AssignTarget(_);
                }
                else
                {
                    Card _ = Heuristic.GetMostValuedMinion(owner, APIHeuristic.Highest_Overall).GetComponent<Card>();

                    SetLogTargetPosition(GameManager.GetInstance().GetOpponentsMinions(owner).IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                    AssignTarget(_);
                }
            }
            else
            {
                AssignTarget(null as Card);
            }
        }
    }

    public override void AssignTarget(Card target)
    {
        SpellObject f = Instantiate(obj, transform.position, Quaternion.identity) as SpellObject;

        f.AssignTarget(target);
        f.AssignPlayer(GameManager.GetInstance().GetOpponent(owner));

        if (GameManager.GetInstance().LOG)
        {
            if (target == null)
                GameManager.GetInstance().LogPlayCard(log__card_position);
            else
                GameManager.GetInstance().LogPlayCardWithTarget(log__card_position, log__target_position);
        }

        GameManager.GetInstance().NotifyAttackIsOver();
        VanishCard();
    } 
}
