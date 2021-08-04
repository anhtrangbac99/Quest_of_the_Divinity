using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public class CrypticLegionSpearmaster : Minion, IWarchief
{
    [SerializeField] private int spear_damage = 2;

    public void ActivateWarchiefEffect()
    {
        List<Card> enemy_minions = GameManager.GetInstance().GetOpponentsMinions(owner);

        if (enemy_minions.Count > 0)
        {
            enemy_minions[Random.Range(0, enemy_minions.Count)].GetComponent<IDamagable>().OnReceiveDamage(null, spear_damage, 0);
        }
    }

    public void DeactivateWarchiefEffect()
    { }

    public override void OnNewTurnUpdate()
    {
        base.OnNewTurnUpdate();
        ActivateWarchiefEffect();
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // As this minion always deals 2 damage to a random enemy minion at beginning of the turn
        return true;
    }
}
