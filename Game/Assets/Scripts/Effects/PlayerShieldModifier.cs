using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShieldModifier : Effect
{
    [SerializeField] private int physical_shield_every_turn = 0;
    [SerializeField] private int spell_shield_every_turn = 0;

    public override void TakeEffect()
    {
        ModifyShields();
    }

    public override void OnNewTurnUpdate()
    {
        base.OnNewTurnUpdate();
        if (activated)
        {
            ModifyShields();
        }
    }

    private void ModifyShields()
    {
        Player player = attached_target.GetComponent<Player>();
        if (player != null)
        {
            player.Physical_shield = physical_shield_every_turn;
            player.Spell_shield = spell_shield_every_turn;
        }
    }
}
