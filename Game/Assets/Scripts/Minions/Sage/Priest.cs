using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Priest : Minion, IChampion, IActivateWithTarget
{
    [SerializeField] private ThrowingHeal particle;
    private ThrowingHeal real_life_particle = null;
    private ThrowingHeal real_life_not_particle = null;

    public void Activate(bool using_gold = true, GameObject target = null)
    {
        log__log_on_activate = false;

        Activate(using_gold);
        GameManager GM = GameManager.GetInstance();

        // if (GM.NON_GRAPHICAL)
            if (target != null)
            {
                Minion _ = target.GetComponent<Minion>();

                SetLogTargetPosition(owner.GetMinionsOnBoard().IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                GM.LogPlayCardWithTarget(log__card_position, log__target_position);

                _.ModifyLife(99999, true);
            }
            else
            {
                Minion _ = Heuristic.GetMostValuedMinion(owner, APIHeuristic.Lowest_Life, true).GetComponent<Minion>();

                SetLogTargetPosition(owner.GetMinionsOnBoard().IndexOf(_) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                GM.LogPlayCardWithTarget(log__card_position, log__target_position);

                _.ModifyLife(99999, true);
            }
    }

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }

    public override void Deactivate(bool returning_gold = true)
    {
        base.Deactivate(returning_gold);
        DeactivateChampionEffect();

    }

    public void ActivateChampionEffect()
    {
        if (owner.GetCardsOnBoard().Count > 1 && !GameManager.GetInstance().NON_GRAPHICAL)
        {
            real_life_particle = Instantiate(particle, transform.position, Quaternion.identity);
            real_life_not_particle = Instantiate(particle, transform.position, Quaternion.identity);
            real_life_not_particle.Offset = Mathf.PI;

            real_life_particle.transform.parent = transform;
            real_life_not_particle.transform.parent = transform;

            GameManager.GetInstance().NotifyCardForceRequireTarget(this, true);
        }
    }

    public void DeactivateChampionEffect()
    {
        if (real_life_particle != null)
        {
            Destroy(real_life_particle);
            Destroy(real_life_not_particle);
        }
    }

    public override void AssignTarget(Minion target)
    {
        real_life_particle.Throw(target, 999999, true, 1f);
        StartCoroutine(WaitAndDestroy());

        if (GameManager.GetInstance().LOG)
        {
            GameManager.GetInstance().LogPlayCardWithTarget(log__card_position, log__target_position);
        }

        GameManager.GetInstance().NotifyAttackIsOver();

        IEnumerator WaitAndDestroy()
        {
            yield return new WaitForSeconds(3f);
            GameObject sub_par = real_life_not_particle.gameObject;

            real_life_not_particle.GetComponent<ParticleSystem>().Stop();            
            Destroy(sub_par, 1.33f);
        }
    }

    public override void AssignTarget(IDamagable target)
    {
        if (real_life_particle) DeactivateChampionEffect();
        base.AssignTarget(target);
    }
}
