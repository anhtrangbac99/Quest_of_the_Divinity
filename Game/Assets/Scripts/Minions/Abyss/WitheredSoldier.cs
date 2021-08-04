using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WitheredSoldier : Minion
{
    public override void Activate(bool using_gold = true)
    {
        base.Activate(using_gold);

        owner.GetDeck().SetRemainingCardStatus(APIDirector.GetInstance().GetCardID(GetCardName()), CardStatus.Unavailable);
    }
}
