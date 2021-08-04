using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardSide : MonoBehaviour
{
    [SerializeField] private int row_invert = 1;
    [SerializeField] private float per_card_distance = 0.5f;
    [SerializeField] private float row_distance = 2.9f;
    [SerializeField] private List<GameObject> cards_on_board = null;
    public Func<GameObject, bool> PlaceCard;
    
    private void Start()
    {
        cards_on_board = new List<GameObject>();
        PlaceCard += PlaceCardToBoard;
    }

    private bool PlaceCardToBoard(GameObject card)
    {
        if (IsFull()) return false;

        if (!cards_on_board.Contains(card))
        {   
            cards_on_board.Add(card);
        }

        // Change card's parent from hand to board side
        card.transform.parent = transform;

        // Sort the card places
        SortCard();
        return true;
    }

    public bool RemoveCardFromBoard(GameObject card, bool sort = true)
    {
        if (cards_on_board.Contains(card))
        {
            cards_on_board.Remove(card);
            if (sort) SortCard();
            return true;
        }

        return false;
    }

    public void ReplaceCard(GameObject chosen, GameObject replacement)
    {
        if (cards_on_board.Contains(chosen))
        {
            int index = cards_on_board.IndexOf(chosen);

            chosen.GetComponent<Minion>().ForceDeath();
            
            if (cards_on_board.Count == 0)
            {
                cards_on_board.Add(replacement);
            }
            else
            {
                cards_on_board.Insert(index, replacement);
            }

            replacement.transform.parent = transform;
            SortCard();
        }
    }
    public void UpdateCardTurn()
    {
        foreach (GameObject card in cards_on_board)
        {
            Minion _minion = card.GetComponent<Minion>();
            if (_minion != null)
            {
                _minion.OnNewTurnUpdate();
            }
        }
    }
    private void SortCard()
    {
        int card_count = cards_on_board.Count;

        Vector3 card_distance = new Vector3(per_card_distance, 0f, 0f);
        Vector3 first_card_place = transform.position - (card_count > 3 ? 2 : card_count - 1) * Vector3.right * 2f;

        int i = 0;
        bool turned = false;
        foreach(GameObject card in cards_on_board)
        {
            card.GetComponent<MinionVisualizer>().Move(first_card_place + i++ * card_distance, 0.33f, true);
            
            if (!turned && i == 3)
            {
                turned = true;
                i = 0;
                first_card_place = transform.position + Vector3.up * row_distance * row_invert - (card_count - 4) * Vector3.right * 2f;
            }
        }
    }

    public List<GameObject> GetCards()
    {
        return cards_on_board;
    }

    public bool IsFull() => cards_on_board.Count == Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD;

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/

    public bool HasFrozen()
    {
        foreach (GameObject _ in cards_on_board)
        {
            if (_.GetComponent<Minion>().IsFreezing())
                return true;
        }

        return false;
    }

    public bool HasDragon(bool notTOAA = false)
    {
        foreach(GameObject _ in cards_on_board)
        {
            if (_.GetComponent<Dragon>() != null)
            {
                if (notTOAA && _.GetComponent<TheOneAboveAll>() == null)
                {
                    return true;
                }
                else if (!notTOAA)
                {
                    return true;
                }
                else
                {
                    continue;
                }
            }
        }

        return false;
    }

    public CardData[] GetCardDatas()
    {
        CardData[] data = new CardData[cards_on_board.Count];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = cards_on_board[i].GetComponent<ICardData>().RetrieveCardData();
        }

        return data;
    }

    public IDamagable GetTarget(int position)
    {
        if (cards_on_board.Count > position)
        {
            return cards_on_board[position].GetComponent<IDamagable>();
        }

        return null;
    }
    public GameObject GetCardObject(int position)
    {
        if (position >= 0 && position < cards_on_board.Count)
        {
            return cards_on_board[position];
        }

        return null;
    }
    public void CardAttack(int position, IDamagable target)
    {
        if (cards_on_board.Count > position)
        {
            Minion _ = cards_on_board[position].GetComponent<Minion>();
            _.AssignTarget(target);
        }
        else
        {
            Debug.Log($"Wrong minion position! {position} - {cards_on_board.Count}");
        }
    }
}
