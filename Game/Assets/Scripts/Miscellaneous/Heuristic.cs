using UnityEngine;

public static class Heuristic
{
    public static GameObject GetMostValuedMinion(Player player, APIHeuristic desired_heuristic = APIHeuristic.Highest_Overall, bool ally = false)
    {
        Card[] minions = (!ally ? GameManager.GetInstance().GetOpponentsMinions(player) : player.GetCardsOnBoard()).ToArray();

        if (minions.Length == 0) return null;

        int heuristic_damage_factor = 1;
        if (desired_heuristic == APIHeuristic.Highest_Life 
            || desired_heuristic == APIHeuristic.Lowest_Life)
        {
            heuristic_damage_factor = 0;
        }
        else if (desired_heuristic == APIHeuristic.Lowest_Damage 
            || desired_heuristic == APIHeuristic.Lowest_Overall)
        {
            heuristic_damage_factor = -1;
        }

        int heuristic_life_factor = 1;

        if (desired_heuristic == APIHeuristic.Highest_Damage 
            || desired_heuristic == APIHeuristic.Lowest_Damage)
        {
            heuristic_life_factor = 0;
        }
        else if (desired_heuristic == APIHeuristic.Lowest_Life 
            || desired_heuristic == APIHeuristic.Lowest_Overall)
        {
            heuristic_life_factor = -1;
        }

        Card chosen_minion = minions[0];
        int argmax_score = compute_score(chosen_minion as Minion);

        foreach(Card _ in minions)
        {
            int _score = compute_score(_ as Minion);
            if ((heuristic_damage_factor > 0 || heuristic_life_factor > 0) && _score > argmax_score)
            {
                argmax_score = _score;
                chosen_minion = _;
            }
            else if ((heuristic_damage_factor < 0 || heuristic_life_factor < 0) && _score < argmax_score)
            {
                argmax_score = _score;
                chosen_minion = _;
            }
        }

        return chosen_minion.gameObject;

        int compute_score(Minion m)
        {
            return heuristic_damage_factor * m.GetDamage() + heuristic_life_factor * m.GetCurrentLife();
        }
    }
}
