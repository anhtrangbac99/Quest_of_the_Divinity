using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAdditionalCard : Effect
{
    [SerializeField] private int num_card = 1;
    public override void TakeEffect()
    {
        Player player = attached_target.GetComponent<Player>();
        player?.RetrieveCard(num_card);
    }

    public override void OnNewTurnUpdate()
    {
        base.OnNewTurnUpdate();
        if (activated)
        {
            TakeEffect();
        }
    }
}
