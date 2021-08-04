using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutcastSage : Minion, IPioneer
{
    [SerializeField] private Attractor attractor;
    [SerializeField] private MinionClass retrieving_card_class = MinionClass.Abyss;

    public override void Death()
    {
        if (!GameManager.GetInstance().NON_GRAPHICAL)
        {
            Attractor a = Instantiate(attractor, transform.position, Quaternion.identity);
            a.Move(owner.GetDeck().transform.position + Vector3.right * 1.5f, 2f);
        }

        base.Death();
    }
    public override void OnDeathCallback()
    {
        base.OnDeathCallback();
        ActivatePioneerEffect();
    }

    public void ActivatePioneerEffect()
    {
        owner.RetrieveCard(retrieving_card_class);
    }
}
