using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WanderingShaman : Minion, IChampion
{
    [SerializeField] private Healing circle = null;
    [SerializeField] private int initial_owner_healing_amount = 1;


    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }
    public void ActivateChampionEffect()
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            owner.Heal(initial_owner_healing_amount);
        }
        else
        {
            Healing h = Instantiate(circle, transform.position, Quaternion.identity);
            h.SetOwner(gameObject);

            h.SetData(1, owner.gameObject, initial_owner_healing_amount);
        }        
    }

    public void DeactivateChampionEffect()
    {

    }
}
