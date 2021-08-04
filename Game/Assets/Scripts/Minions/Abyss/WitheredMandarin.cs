using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitheredMandarin : Minion, IChampion
{
    [SerializeField] private Card withered_soldier = null;

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
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
            yield return new WaitForSeconds(0.5f);
            Spawn();
        }

        void Spawn()
        {
            if (owner.CanPlaceCard())
            {
                Minion soldier = Instantiate(withered_soldier, Vector3.zero, transform.rotation) as Minion;

                soldier.SetOwner(owner);
                soldier.Activate(false);
            }
        }
    }

    public void DeactivateChampionEffect()
    {

    }
}
