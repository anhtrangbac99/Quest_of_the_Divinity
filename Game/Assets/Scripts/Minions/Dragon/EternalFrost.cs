using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EternalFrost : Dragon, IChampion
{
    [SerializeField] private Effect eternal_frost__freezing_effect = null;
    [SerializeField] private EternalFrostIceShard shard;
    [SerializeField] private int num_freezing_enemy = 2;
    [SerializeField] private int additional_damage_on_freezing_enemy = 1;

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();

        if (is_first_turn)
        {
            ActivateChampionEffect();
        }
    }

    public void ActivateChampionEffect()
    {
        List<Card> enemy_minions = GameManager.GetInstance().GetOpponentsMinions(owner);

        if (enemy_minions.Count > 0)
        {
            int chosen_one = -1;
            int loop_count = 0;
            for (int i = 0; i < num_freezing_enemy; i++)
            {
                int chosen = Random.Range(0, enemy_minions.Count);
                if (chosen != chosen_one)
                {
                    chosen_one = chosen;
                }
                else if (++loop_count <= 5)
                {
                    continue;
                }

                EternalFrostIceShard s = Instantiate(shard, transform.position, Quaternion.identity);
                s.Throw(enemy_minions[chosen] as Minion, eternal_frost__freezing_effect, 2.5f);
            }
        }
    }

    public void DeactivateChampionEffect()
    {

    }

    public override void Strike(bool duel = false)
    {
        Minion _ = target as Minion;
        bool added = false;

        if (_ != null)
        {
            if(_.IsFreezing())
            {
                added_physical_damage += additional_damage_on_freezing_enemy;
                added = true;
            }
            _.AddEffect(eternal_frost__freezing_effect);
        }

        base.Strike(duel);

        if (added)
        {
            added_physical_damage -= additional_damage_on_freezing_enemy;
        }
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // If the other boardside has a frozen enemy, then true, else false
        return GameManager.GetInstance().GetOpponentBoardSide(owner).HasFrozen();
    }
}
