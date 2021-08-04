using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DancingFlame : SpellObject
{
    [SerializeField] private int chain_time = 2;
   
    private List<Card> damaged = null;
   
    public override void AssignTarget(Card card)
    {
        card.GetComponent<IDamagable>().OnReceiveDamage(null, 0, spell_damage);

        damaged = new List<Card>();
        damaged.Add(card);
    }

    public override void AssignPlayer(Player player)
    {
        this.player = player;
        List<Card> enemies = player.GetCardsOnBoard();
        if (enemies.Count >= 1)
        {
            Chain(chain_time);
        }

        Destroy(gameObject, 0.5f);
    }

    private void Chain(int chain_time)
    {
        List<Card> enemies = player.GetCardsOnBoard();
        if (chain_time <= 0 || enemies.Count == 0) return;

        int chosen = -1;
        for (int i = 0; i < 10; i++)
        {
            chosen = Random.Range(0, enemies.Count);
            if (!damaged.Contains(enemies[chosen]))
            {
                break;
            }
        }

        if (damaged.LastIndexOf(enemies[chosen]) == damaged.Count - 1) return;

        enemies[chosen].GetComponent<IDamagable>().OnReceiveDamage(null, 0, spell_damage);

        damaged.Add(enemies[chosen]);

        Chain(--chain_time);
    }
}
