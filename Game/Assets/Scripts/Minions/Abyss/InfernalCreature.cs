using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfernalCreature : Minion
{
    public Action death_notifier;

    public override void Activate(bool using_gold = true)
    {
        base.Activate(using_gold);

        owner.GetDeck().SetRemainingCardStatus(APIDirector.GetInstance().GetCardID(GetCardName()), CardStatus.Unavailable);
    }
    public override void Death()
    {
        if (death_notifier != null)
        {
            death_notifier();
        }
        base.Death();
    }
}
