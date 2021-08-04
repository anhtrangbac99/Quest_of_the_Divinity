using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceGoldLimit : Effect
{
    [SerializeField] private int amount_of_modify = 3;
    public override void TakeEffect()
    {
        Player player = attached_target.GetComponent<Player>();
        player?.ModifyCurrentMaximumGoldLimit(-amount_of_modify);
    }

    public override void RevertEffect()
    {
        Player player = attached_target.GetComponent<Player>();

        player?.ModifyCurrentMaximumGoldLimit(amount_of_modify);
        base.RevertEffect();
    }
}
