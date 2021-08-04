using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonflyArcher : Minion, IChampion
{
    [SerializeField] private ThrowingWeapon arrow;
    [SerializeField] private int initial_damage = 1;

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }
    public void ActivateChampionEffect()
    {
        ThrowingWeapon t = Instantiate(arrow, transform.position, Quaternion.identity);
        t.Throw(GameManager.GetInstance().GetOpponent(owner), initial_damage, 0, 2.5f);
    }

    public void DeactivateChampionEffect()
    {

    }
}
