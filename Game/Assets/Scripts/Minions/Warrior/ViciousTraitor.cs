using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViciousTraitor : Minion, IPioneer
{
    [SerializeField] private int on_death_damage = 3;
    public void ActivatePioneerEffect()
    {
        target = owner;
        visualizer.AttackMove(owner.transform.position, attack_time);
    }

    public override void Strike(bool duel = false)
    {
        if (!IsDead())
        {
            base.Strike(duel);
        }
        else
        {
            target.OnReceiveDamage(null, on_death_damage, 0);
            base.Death();
        }
    }

    public override void Death()
    {
        Invoke("ActivatePioneerEffect", 0.33f);
    }
}
