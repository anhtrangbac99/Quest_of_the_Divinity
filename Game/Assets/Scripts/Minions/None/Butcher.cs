using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Butcher : Minion, IPioneer
{
    [SerializeField] private Marker mark;

    private IDamagable last_target = null;
    [SerializeField] private int damage_to_last_target = 2;


    public override void OnDeathCallback()
    {
        base.OnDeathCallback();
        ActivatePioneerEffect();
    }

    public void ActivatePioneerEffect()
    {
        Minion _ = last_target as Minion;

        if (mark.IsDrawing())
        {
            if (_ != null)
            {
                if (!_.IsDead())
                {
                    mark.StopDrawing();
                    last_target.OnReceiveDamage(null, damage_to_last_target, 0);
                }                
            }
            else
            {
                last_target.OnReceiveDamage(null, damage_to_last_target, 0);
            }
        }
    }

    public override void Strike(bool duel = false)
    {
        last_target = target;
        MonoBehaviour _ = last_target as MonoBehaviour;

        mark.SetPositions(transform.gameObject, _.gameObject);

        base.Strike(duel);
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        if (last_target == null) return false;

        bool target_dead = false;

        // Only process if last target was a minion, else the effect will always be available
        Minion _ = last_target as Minion;
        if (_ != null && _.IsDead())
        {
            target_dead = true;
        }

        return !target_dead;
    }
}
