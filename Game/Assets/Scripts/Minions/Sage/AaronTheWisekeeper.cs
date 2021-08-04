using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AaronTheWisekeeper : Minion, IWarchief, IPioneer
{
    [SerializeField] private GameObject shine;
    [SerializeField] private int lives = 1;
    [SerializeField] private int return_lives_on_death = 5;
    [SerializeField] private int additional_damage_per_turn = 2;

    public void ActivateWarchiefEffect()
    {
        ModifyDamage(additional_damage_per_turn);
    }

    public void DeactivateWarchiefEffect()
    {

    }

    public void ActivatePioneerEffect()
    {
        if (!GameManager.GetInstance().NON_GRAPHICAL)
        {
            shine.SetActive(true);
        }

        lives--;
        ModifyLife(return_lives_on_death, true);
    }

    public override void Death()
    {
        if (lives < 0)
        {
            base.Death();
        }
        else
        {
            ActivatePioneerEffect();
        }
    }

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
        // As every turn, this minion gains 2 additional damage.
        return true;
    }
}
