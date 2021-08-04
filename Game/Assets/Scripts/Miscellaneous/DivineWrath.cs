using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivineWrath : SpellObject
{
    [SerializeField] private int repeat_time_on_player = 5;
    [SerializeField] private int repeat_time_on_minion = 8;

    public override void AssignTarget(Card card)
    {

    }

    public override void AssignPlayer(Player player)
    {
        this.player = player;
        
        List<Card> enemies = player.GetCardsOnBoard();

        int strike_times = (enemies.Count > 0 ? repeat_time_on_minion : repeat_time_on_player);
        Strike(strike_times, (strike_times == repeat_time_on_player));
    }

    private void Strike(int times, bool on_player)
    {
        if (times <= 0) return;

        if (on_player)
        {
            player.GetComponent<IDamagable>().OnReceiveDamage(null, 0, spell_damage);
        }
        else
        {
            List<Card> enemies = player.GetCardsOnBoard();
            if (enemies.Count == 0) return;

            int chosen = Random.Range(0, enemies.Count);

            enemies[chosen].GetComponent<IDamagable>().OnReceiveDamage(null, 0, spell_damage);
            
        }

        Strike(--times, on_player);
    }
}
