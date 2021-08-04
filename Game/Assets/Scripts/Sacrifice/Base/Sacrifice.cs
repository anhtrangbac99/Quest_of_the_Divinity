using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Sacrifice : Card, IActivateWithTarget
{
    [SerializeField] protected int minion_card_min_cost = 1;
    [SerializeField] protected int flat_damage_to_user = 0;

    [SerializeField] protected int number_of_minions_to_activate = 1;

    [SerializeField][Range(0f, 1f)] protected float percentage_damage_to_user = 0.5f;

    [SerializeField] protected Effect effect_reference = null;

    protected new SacrificeVisualizer visualizer;

    [SerializeField] protected TargetType target_type = TargetType.Minion;

    protected virtual void TakeEffect()
    {
        visualizer.ToggleFront(true);

        StartCoroutine(WaitAndVanish(1f));

        IEnumerator WaitAndVanish(float time)
        {
            yield return new WaitForSeconds(time);

            gameObject.SetActive(false);
        }
    }

    protected override void Start()
    {
        base.Start();

        visualizer.SetName(card_name_in_string);
        visualizer.SetCost(cost);
    }

    public override void Activate(bool using_gold = true)
    {
        if (owner.GetCardsOnBoard().Count < number_of_minions_to_activate || !owner.PlaceThisCard(this, false)) 
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
            visualizer.OnActivateSetUp();

            activated = true;

            transform.localScale = new Vector3(1, 1, 0.01f);

            GameManager.GetInstance().NotifySacrificeRequireTarget(this, owner);
        }
    }

    public void Activate(bool using_gold = true, GameObject target = null)
    {
        if (owner.GetCardsOnBoard().Count < number_of_minions_to_activate || !owner.PlaceThisCard(this, false))
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
            visualizer.OnActivateSetUp();

            activated = true;

            transform.localScale = new Vector3(1, 1, 0.01f);

            GameManager GM = GameManager.GetInstance();

            GM.NotifySacrificeRequireTarget(this, owner);

            Card card_target = target.GetComponent<Card>();

            GM.NotifyCardIsClicked(card_target, card_target.GetOwner());
        }
    }

    public override void Deactivate(bool returning_gold = true)
    {
        owner.AddCardToHand(this);
        if (returning_gold)
        {
            owner.AddGold(cost);
        }

        activated = false;
    }
    public TargetType GetSacrificeTargetType() => target_type;

    ///<summary>
    /// Assign the target for this sacrifice card.
    /// <para>Parameters:</para>
    /// <para>card<param name="card"/>: the target card.</para>
    ///</summary>
    public virtual void AssignTarget(Card card)
    {
        Minion minion = card as Minion;
        // Damage from minion = Floor(damage_percentage * cost)
        int damage_from_minion = (int)Math.Floor(percentage_damage_to_user * minion.GetCost());

        // Final damage = flat_damage + damage_from_minion
        int final_spell_damage = flat_damage_to_user + damage_from_minion;
        owner.GetComponent<IDamagable>().OnReceiveDamage(null, 0, final_spell_damage);

        // Destroy the chosen minion card
        minion.Death();

        // After dealing damage to the player, take effect
        TakeEffect();

        if (GameManager.GetInstance().LOG)
        {
            GameManager.GetInstance().LogPlayCardWithTarget(log__card_position, log__target_position);
        }

        // And notify the turn is over
        GameManager.GetInstance().NotifyAttackIsOver();

        owner.UpdateCardStatus(APIDirector.GetInstance().GetCardID(GetCardName()), UsedCardStatus.Used);
    }

    public int GetMinimumMinionCost() => minion_card_min_cost;

    protected override void SetVisualizer()
    {
        visualizer = GetComponent<SacrificeVisualizer>();
    }

    public bool IsActivated() => activated;

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/

    public override CardData RetrieveCardData()
    {
        int i = APIDirector.GetInstance().GetCardID(card_name_in_string);

        return new CardData(false, i);
    }
}
