using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Effect : MonoBehaviour, ITurnUpdatable
{
    [SerializeField] protected string effect_id = "";
    [SerializeField] protected int maximum_turns = 1; 

    [SerializeField] protected int remaining_turns = 1;

    [SerializeField] protected EffectTargetType target_type = EffectTargetType.Minion;
    protected GameObject attached_target = null;

    protected bool activated = false;
    /// <summary>
    /// Reset this effect expiring time to a chosen number, can be at its own defined maximum turns.
    /// <para>
    /// Parameters:
    /// </para>
    /// <para>to_max<param name="to_max">: Reseting this effect's remaining turn to max or not. </param> </para>
    /// <para>num_turn<param name="num_turn">: If to_max is false, how many turn you want to reset this effect to. </param> </para>
    /// </summary>
    public void ResetRemainingTurn(bool to_max = true, int num_turn = 1)
    {
        if (!activated)
        {
            activated = true;
            remaining_turns = num_turn;
            TakeEffect();
        }

        if (to_max)
        {
            remaining_turns = maximum_turns;
        }
        else if (num_turn >= 1)
        {
            remaining_turns = num_turn;
        }
        else
        {
            Debug.LogAssertion("The number of turns when reseting " + this.ToString() + " must be greater than 1. (value: " + num_turn + ").");
        }
    }
     
    public abstract void TakeEffect();
    public virtual void TakeEffect(Card card) {}
    public virtual void RevertEffect() 
    {
        activated = false;
    }

    protected void DecreaseTurn() => --remaining_turns;
    
    public void AssignTarget(GameObject target)
    {
        attached_target = target;
        activated = true;
        TakeEffect();
    }

    public bool IsActivating() => activated;
    public string GetID() => effect_id;


    /****************************
    *                           *
    *   ITURNUPDATABLE SECTION  *
    *                           *
    *****************************/

    public void RegisterTurnUpdatable() { }

    public void UnregisterTurnUpdatable() { }

    public virtual void OnNewTurnUpdate()
    {
        if (activated)
        {
            DecreaseTurn();
            if (remaining_turns < 0)
            {
                RevertEffect();
            }   
        }
    }

}
