using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetrayedShapeshifter : Minion, IPioneer
{
    [SerializeField] private Card raven = null;
    [SerializeField] private int num_ravens = 2;

    public override void OnDeathCallback()
    {
        ActivatePioneerEffect();
        base.OnDeathCallback();
    }

    public void ActivatePioneerEffect()
    {
        for (int i = 0; i < num_ravens && owner.CanPlaceCard(); i++)
        {
            Minion r = Instantiate(raven) as Minion;
            r.SetOwner(owner);
            r.Activate(false);
        }
    }
}
