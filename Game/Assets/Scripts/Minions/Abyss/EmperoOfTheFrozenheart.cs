using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmperoOfTheFrozenheart : Minion, IPioneer
{
    [SerializeField] private EternalFrostIceShard shard;
    [SerializeField] private Effect emperor__freezing_effect = null;

    public override void OnDeathCallback()
    {
        base.OnDeathCallback();
        ActivatePioneerEffect();
    }

    public void ActivatePioneerEffect()
    {
        List<Card> opponent_cards = GameManager.GetInstance().GetOpponentsMinions(owner);

        if (opponent_cards.Count > 0)
        {
            EternalFrostIceShard s = Instantiate(shard, transform.position, Quaternion.identity);

            s.Throw(opponent_cards[Random.Range(0, opponent_cards.Count)] as Minion, emperor__freezing_effect, 2.5f);
        }
    }
}
