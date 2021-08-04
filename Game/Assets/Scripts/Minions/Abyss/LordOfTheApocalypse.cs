using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LordOfTheApocalypse : Minion, IChampion, IWarchief
{
    [SerializeField] GameObject infernal_particle;
    private GameObject real_life_particle = null;
    [SerializeField] private Card infernal_creature = null;
    [SerializeField] private int num_infernal_creature = 2;

    [SerializeField] private int additional_damage_on_creature_death = 2;
    [SerializeField] private int additional_life_on_creature_death = 2;
    private int num_creature_dead = 0;

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();

        ActivateChampionEffect();

        if (!GameManager.GetInstance().NON_GRAPHICAL)
            real_life_particle = Instantiate(infernal_particle, transform);
    }

    public override void OnDeathCallback()
    {
        base.OnDeathCallback();

        if (real_life_particle != null)
        {
            Destroy(real_life_particle);
        }
    }

    public void ActivateChampionEffect()
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            Spawn();
        }
        else
        {
            StartCoroutine(WaitAndSpawn());
        }

        IEnumerator WaitAndSpawn()
        {
            yield return new WaitForSeconds(1.5f);
            Spawn();
        }

        void Spawn()
        {
            for (int i = 0; i < num_infernal_creature && owner.CanPlaceCard(); i++)
            {
                Minion _ = Instantiate(infernal_creature, Vector3.zero, transform.rotation) as Minion;

                _.SetOwner(owner);
                _.Activate(false);
                _.GetComponent<InfernalCreature>().death_notifier += ActivateWarchiefEffect;
            }
        }
    }

    public void DeactivateChampionEffect()
    {

    }

    public void ActivateWarchiefEffect()
    {
        ModifyDamage(additional_damage_on_creature_death);
        ModifyLife(additional_life_on_creature_death);
        base_maximum_life += additional_life_on_creature_death;

        if (!GameManager.GetInstance().NON_GRAPHICAL)
        {
            var emission = real_life_particle.GetComponentInChildren<ParticleSystem>().emission;
            emission.rateOverTime = 5 + ++num_creature_dead * 5;
        }
    }

    public void DeactivateWarchiefEffect()
    {

    }
}
