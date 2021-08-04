using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Spell : Card
{
    [SerializeField] protected int physical_damage = 0;
    [SerializeField] protected int spell_damage = 0;
    
    [SerializeField] protected TargetSide side = TargetSide.Enemy;
    [SerializeField] protected TargetType type = TargetType.Minion;
    public override void Activate(bool using_gold = true)
    {
        if (!owner.PlaceThisCard(this, false))
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
            visualizer.OnActivateSetUp();
            visualizer.ToggleFront(true);

            activated = true;

            transform.localScale = new Vector3(1, 1, 0.01f);

            GameManager.GetInstance().NotifySpellRequireTarget(this, owner);
        }
    }

    protected override void Start()
    {
        base.Start();

        visualizer.SetName(card_name_in_string);
        visualizer.SetCost(cost);
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
    
    public virtual void AssignTarget(Card target)
    { 
        if (GameManager.GetInstance().LOG)
        {
            GameManager.GetInstance().LogPlayCardWithTarget(log__card_position, log__target_position);
        }
    }

    public virtual void AssignTarget(Player target)
    {  }

    public TargetSide GetSpellCardTargetSide()
    {
        return side;
    }

    public TargetType GetSpellCardTargetType()
    {
        return type;
    }

    protected void VanishCard()
    {
        owner.UpdateCardStatus(APIDirector.GetInstance().GetCardID(GetCardName()), UsedCardStatus.Used);

        StartCoroutine(WaitAndVanish(1f));

        IEnumerator WaitAndVanish(float time)
        {
            yield return new WaitForSeconds(time);

            gameObject.SetActive(false);
        }
    }

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
