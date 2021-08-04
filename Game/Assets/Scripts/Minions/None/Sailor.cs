using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sailor : Minion, IChampion
{
    [SerializeField] private ThrowingWeapon dagger = null;
    [SerializeField] private int initial_damage = 1;

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }
    public void ActivateChampionEffect()
    {
        List<Card> enemies = GameManager.GetInstance().GetOpponentsMinions(owner);

        IDamagable target;

        if (enemies.Count > 0)
        {
            target = enemies[Random.Range(0, enemies.Count)].GetComponent<IDamagable>();
        }
        else
        {
            target = GameManager.GetInstance().GetOpponent(owner).GetComponent<IDamagable>();
        }

        ThrowingWeapon throwing_dagger = Instantiate(dagger, transform.position, Quaternion.identity);
        throwing_dagger.Throw(target, initial_damage, 0, 2.5f);
    }

    public void DeactivateChampionEffect()
    {

    }
}
