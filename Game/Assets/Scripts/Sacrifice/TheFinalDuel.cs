using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheFinalDuel : Sacrifice
{
    private Card minion1 = null;
    private Card minion2 = null;
    [SerializeField] private Effect sage_effect = null;
    [SerializeField] private Effect warrior_effect = null;
    [SerializeField] private Effect abyss_effect = null;
    [SerializeField] private Effect nature_effect = null;
    [SerializeField] private Effect dragon_effect = null;

    public override void Activate(bool using_gold = true)
    {
        if (
            (owner.GetCardsOnBoard().Count < 1 
            && GameManager.GetInstance().GetOpponent(owner).GetCardsOnBoard().Count < 1)
        || !owner.PlaceThisCard(this, false, using_gold))
        {
            visualizer.OnEndDrag(null);
        }
        else
        {
            minion1 = null;
            minion2 = null;
            GameManager.GetInstance().NotifySacrificeRequireTargetBoth(this, owner);
        }
    }

    public override void AssignTarget(Card card)
    {
        if (minion1 == null)
        {
            minion1 = card;
        }
        else
        {
            minion2 = card;
            if (minion1.GetOwner() == owner)
            {
                minion1.GetComponent<Minion>().FinalDuel(this, minion2 as Minion);

                if (GameManager.GetInstance().LOG)
                {
                    int minion1_position = owner.GetMinionsOnBoard().IndexOf(minion1 as Minion) + Constants.LOG__CHOOSING_CARD_OFFSET;
                    int minion2_position = GameManager.GetInstance().GetOpponentsMinions(owner).IndexOf(minion2 as Minion) + Constants.LOG__CHOOSING_CARD_OFFSET;

                    GameManager.GetInstance().LogPlayCardWithTarget(log__card_position, minion1_position.ToString() + minion2_position.ToString());
                }
            }
            else
            {
                minion2.GetComponent<Minion>().FinalDuel(this, minion1 as Minion);

                if (GameManager.GetInstance().LOG)
                {
                    int minion1_position = GameManager.GetInstance().GetOpponentsMinions(owner).IndexOf(minion1 as Minion) + Constants.LOG__CHOOSING_CARD_OFFSET;

                    int minion2_position = owner.GetMinionsOnBoard().IndexOf(minion2 as Minion) + Constants.LOG__CHOOSING_CARD_OFFSET;

                    GameManager.GetInstance().LogPlayCardWithTarget(log__card_position, minion2_position.ToString() + minion1_position.ToString());
                }
            }

            owner.UpdateCardStatus(APIDirector.GetInstance().GetCardID(GetCardName()), UsedCardStatus.Used);
        }
    }

    public void ActivateEffectOnwinnerSide(Minion winner)
    {
        Debug.Log("And The Winner Is: " + winner.ToString());
        MinionClass winner_class = winner.GetClass();
        Player won_player = winner.GetOwner();

        switch(winner_class)
        {
            case MinionClass.Sage:
                won_player.AddEffectForPlayer(sage_effect);
                break;
            case MinionClass.Warrior:
                won_player.AddEffectForMinion(warrior_effect);
                break;
            case MinionClass.Abyss:
                won_player.AddEffectForPlayer(abyss_effect);
                break;
            case MinionClass.Nature:
                won_player.AddEffectForPlayer(nature_effect);
                break;
            case MinionClass.Dragon:
                won_player.AddEffectForPlayer(dragon_effect);
                break;
        }

        TakeEffect();

        GameManager.GetInstance().NotifyAttackIsOver();
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/

    public void Activate(bool using_gold, GameObject minion1, GameObject minion2)
    {
        Activate(using_gold);

        // Parsing cards
        Card card_target1 = minion1.GetComponent<Card>();
        Card card_target2 = minion2.GetComponent<Card>();

        // Assigning cards to the final duel
        GameManager.GetInstance().NotifyCardIsClicked(card_target1, card_target1.GetOwner());
        GameManager.GetInstance().NotifyCardIsClicked(card_target2, card_target2.GetOwner());
    }
}
