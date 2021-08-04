using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncarnationOfDeath : Minion, IPioneer, IWarchief
{
    [SerializeField] private ExplodingAttractor energy_effect;
    [SerializeField] private int additional_damage_on_minion_death = 1;

    public override void OnActivateCallback()
    {
        GameManager.GetInstance().minion_dies_notifier += GainDamageOnMinionDeath;
        base.OnActivateCallback();
    }

    public override void OnDeathCallback()
    {
        GameManager.GetInstance().minion_dies_notifier -= GainDamageOnMinionDeath;
        ActivatePioneerEffect();
        base.OnDeathCallback();
    }

    public void ActivateWarchiefEffect()
    {
        ModifyDamage(additional_damage_on_minion_death);
    }

    public void DeactivateWarchiefEffect()
    {

    }

    public void ActivatePioneerEffect()
    {
        ExplodingAttractor e = Instantiate(energy_effect, transform.position, Quaternion.identity);

        Player opponent = GameManager.GetInstance().GetOpponent(owner);
        e.SetTarget(opponent.GetComponent<IDamagable>(), opponent.transform.position, added_physical_damage, 0);
    }

    private void GainDamageOnMinionDeath(Player owner)
    {
        if (owner != this.owner)
        {
            ActivateWarchiefEffect();
        }
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // This minion gains damage on enemies' deaths
        return true;
    }
}
