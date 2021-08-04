using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alchemist : Minion, IChampion, IActivateWithTarget
{
    [SerializeField] private GameObject circle;
    [SerializeField] private GameObject particle;
    private GameObject real_life_circle = null;
    [SerializeField] private int additional_damage = 1;
    [SerializeField] private int additional_life = 2;

    public void Activate(bool using_gold = true, GameObject target = null)
    {
        log__log_on_activate = false;

        Activate(using_gold);

        // if (GameManager.GetInstance().NON_GRAPHICAL)
            if (target == null)
            {
                Minion _ = Heuristic.GetMostValuedMinion(owner, APIHeuristic.Highest_Overall, true).GetComponent<Minion>();

                SetLogTargetPosition(owner.GetMinionsOnBoard().IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                AssignTarget(_);
            }
            else
            {
                Minion _ = target.GetComponent<Minion>();

                SetLogTargetPosition(owner.GetMinionsOnBoard().IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                AssignTarget(_);
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
        if (!GameManager.GetInstance().NON_GRAPHICAL)
        {
            real_life_circle = Instantiate(circle, transform.position, transform.rotation);
            GameManager.GetInstance().NotifyCardForceRequireTarget(this, true);
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

    public override void AssignTarget(Minion target)
    {
        GameManager GM = GameManager.GetInstance();

        if (GM.NON_GRAPHICAL)
        {
            Alchemy();
        }
        else
        {
            GM.switch_off();

            Instantiate(particle, target.transform.position, Quaternion.identity);
            DeactivateChampionEffect();

            StartCoroutine(WaitAndActivate());
            GM.NotifyAttackIsOver();

            IEnumerator WaitAndActivate()
            {
                yield return new WaitForSeconds(1.75f);

                Alchemy();

                GM.switch_on();
            }
        }
        
        if (GM.LOG)
        {
            GM.LogPlayCardWithTarget(log__card_position, log__target_position);
        }

        void Alchemy()
        {
            target.ModifyDamage(additional_damage);
            target.ModifyLife(additional_life);
            target.AddGuardAttribute();
        }
    }

    public override void AssignTarget(IDamagable target)
    {
        if (real_life_circle) DeactivateChampionEffect();
        base.AssignTarget(target);
    }
}
