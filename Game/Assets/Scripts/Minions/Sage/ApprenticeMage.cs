using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApprenticeMage : Minion, IChampion, IActivateWithTarget
{
    [SerializeField] private GameObject circle;
    [SerializeField] private ThrowingWeapon magic_ball;
    [SerializeField] private int initial_damage_to_enemy_minion = 2;
    private GameObject real_life_circle = null;

    public void Activate(bool using_gold = true, GameObject target = null)
    {
        log__log_on_activate = false;

        Activate(using_gold);

        // if (GameManager.GetInstance().NON_GRAPHICAL)
            if (target != null)
            {
                SetLogTargetPosition(GameManager.GetInstance().GetOpponentsMinions(GetOwner()).IndexOf(target.GetComponent<Card>()) + Constants.LOG__TARGET_OPPONENT_CARD_OFFSET);

                AssignTarget(target.GetComponent<IDamagable>());
            }
            else
            {
                if (GameManager.GetInstance().GetOpponentsMinions(owner).Count > 0)
                {
                    GameObject _ = Heuristic.GetMostValuedMinion(owner, APIHeuristic.Lowest_Life);

                    SetLogTargetPosition(GameManager.GetInstance().GetOpponentsMinions(GetOwner()).IndexOf(target.GetComponent<Card>()) + Constants.LOG__TARGET_OPPONENT_CARD_OFFSET);

                    AssignTarget(_.GetComponent<IDamagable>());
                }
                is_first_turn = false;
            }
    }

    public override void Deactivate(bool returning_gold = true)
    {
        base.Deactivate(returning_gold);
        DeactivateChampionEffect();

    }
    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }

    public void ActivateChampionEffect()
    {
        GameManager GM = GameManager.GetInstance();

        if (GM.GetOpponentsMinions(owner).Count > 0 && !GM.NON_GRAPHICAL)
        {
            GM.NotifyCardForceRequireTarget(this);
            real_life_circle = Instantiate(circle, transform.position, transform.rotation);
            real_life_circle.transform.parent = transform;
        }
    }

    public void DeactivateChampionEffect()
    {
        if (real_life_circle != null)
        {
            Destroy(real_life_circle);
        }
    }

    public override void AssignTarget(IDamagable target)
    {
        if (is_first_turn)
        {
            GameManager GM = GameManager.GetInstance();

            DeactivateChampionEffect();
            if (!GM.NON_GRAPHICAL)
            {
                ThrowingWeapon t = Instantiate(magic_ball, transform.position, Quaternion.identity);
                t.Throw(target, 0, initial_damage_to_enemy_minion, 1.5f);
            }
            else
            {
                target.OnReceiveDamage(null, 0, initial_damage_to_enemy_minion);
            }

            is_first_turn = false;

            if (GM.LOG)
            {
                GM.LogPlayCardWithTarget(log__card_position, log__target_position);
            }

            GM.NotifyAttackIsOver();
        }
        else
        {
            if (real_life_circle) DeactivateChampionEffect();
            base.AssignTarget(target);
        }
    }
}
