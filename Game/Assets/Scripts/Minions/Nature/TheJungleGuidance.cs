using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheJungleGuidance : Minion, IChampion, IActivateWithTarget
{
    [SerializeField] private ThrowingHeal heal;
    private ThrowingHeal real_life_heal = null;

    [SerializeField] private int initial_healing_amount = 2;

    public void Activate(bool using_gold = true, GameObject target = null)
    {
        log__log_on_activate = false;

        Activate(using_gold);

        // if (GameManager.GetInstance().NON_GRAPHICAL)
            if (target != null)
            {
                Minion _ = target.GetComponent<Minion>();

                SetLogTargetPosition(owner.GetMinionsOnBoard().IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                GameManager.GetInstance().LogPlayCardWithTarget(log__card_position, log__target_position);

                _.ModifyLife(initial_healing_amount, true);
            }
            else
            {
                Minion _ = Heuristic.GetMostValuedMinion(owner, APIHeuristic.Lowest_Life, true).GetComponent<Minion>();

                SetLogTargetPosition(owner.GetMinionsOnBoard().IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                GameManager.GetInstance().LogPlayCardWithTarget(log__card_position, log__target_position);

                _.ModifyLife(initial_healing_amount, true);
            }
    }

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }

    public override void Deactivate(bool returning_gold = true)
    {
        DeactivateChampionEffect();
        base.Deactivate(returning_gold);
    }
    public void ActivateChampionEffect()
    {
        if (owner.GetCardsOnBoard().Count > 1 && !GameManager.GetInstance().NON_GRAPHICAL)
        {
            GameManager.GetInstance().NotifyCardForceRequireTarget(this, true);
            real_life_heal = Instantiate(heal, transform.position, Quaternion.identity, transform);
        }
    }

    public void DeactivateChampionEffect()
    {
        if (real_life_heal != null)
        {
            Destroy(real_life_heal);
        }
    }

    public override void AssignTarget(Minion target)
    {
        real_life_heal.Throw(target, initial_healing_amount, true, 2f);

        GameManager.GetInstance().LogPlayCardWithTarget(log__card_position, log__target_position);
        GameManager.GetInstance().NotifyAttackIsOver();
    }

    public override void AssignTarget(IDamagable target)
    {
        if (real_life_heal) DeactivateChampionEffect();
        base.AssignTarget(target);
    }
}
