using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piggy : Minion
{
    public override void Activate(bool using_gold = true)
    {
        // Set this minion's current life to its base maximum life
        life = base_maximum_life;

        // Set base stats
        critical_chance = 0;
        added_physical_damage = 0;
        added_spell_damage = 0;
        added_life = 0;

        // Initialize the effect list
        effects = new Dictionary<string, Effect>();

        // Raider can attack immediately
        if (is_raider) wait_turns = 0;
        else wait_turns = 1;      

        if (is_guard)
        {
            owner.AddNewGuard(this);
        }

        if (visualizer == null)
        {
            SetVisualizer();
        }

        visualizer.OnActivateSetUp();
        visualizer.ChangeCardAppearance(true, false);
        ShowDamageAndLife();
        
        // Raise the activation flag
        activated = true;
        
        owner.ApplyAllEffect(this);

        owner.GetDeck().SetRemainingCardStatus(APIDirector.GetInstance().GetCardID(GetCardName()), CardStatus.Unavailable);
    }
}
