using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : MonoBehaviour, ICardData
{
    public int Log_CardPosition
    {
        get { return log__card_position; }
        set { log__card_position = value; }
    }

    protected bool activated = false;
    [SerializeField] protected int cost = 0;

    protected int log__card_position;
    protected int log__target_position;

    [SerializeField] protected string card_name_in_string = "";
    protected IDamagable target = null;
    protected Player owner = null;

    protected Visualizer visualizer = null;

    protected virtual void Start()
    {
        SetVisualizer();
    }

    ///<summary> 
    /// Called once when this minion is activated on the battlefield.
    /// </summary>
    public abstract void Activate(bool using_gold = true);

    public abstract void Deactivate(bool returning_gold = true);

    //-------------------------------------------------
    /// <summary>
    /// Retrieve this card's data at the moment
    /// </summary>
    /// <returns>A CardData instance represents this card's informations. </returns>
    public abstract CardData RetrieveCardData();

    ///<summary> 
    /// Assign a IDamagable target to this card.
    /// </summary>
    public virtual void AssignTarget(IDamagable target)
    {
        this.target = target;
    }

    ///<summary> 
    /// Assign a Minion target to this card.
    /// </summary>
    public virtual void AssignTarget(Minion target) {}
    //--------------------------------------------------
    public void SetOwner(Player owner)
    {
        this.owner = owner;
    }
    public Player GetOwner() => owner;
    public string GetCardName() => card_name_in_string;
    public int GetCost() => cost;
    
    //-------------------------------------------------
    protected virtual void SetVisualizer()
    {
        visualizer = GetComponent<Visualizer>();
    }

    //-------------------------------------------------
    public void SetLogTargetPosition(int position)
    {
        log__target_position = position;
    }
}
