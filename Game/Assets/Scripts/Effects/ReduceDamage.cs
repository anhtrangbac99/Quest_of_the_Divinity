using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceDamage : Effect
{
    [SerializeField] private GameObject heart_particle;
    private GameObject real_life_particle = null;
    [SerializeField] private int value = 1;
    public override void TakeEffect()
    {
        if (target_type == EffectTargetType.Minion)
        {
            Minion minion = attached_target.GetComponent<Minion>();

            minion.ModifyDamage(-value);
            if (heart_particle != null)
            {
                real_life_particle = Instantiate(heart_particle, minion.transform);
            }
        }
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        if (target_type == EffectTargetType.Minion)
        {
            Minion minion = attached_target.GetComponent<Minion>();

            minion.ModifyDamage(value);
            Destroy(real_life_particle);
        }
    }
}
