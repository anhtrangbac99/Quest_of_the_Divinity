using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour, IMouseFocusHandler, ISwitch
{
    private bool is_focusing = false;
    [SerializeField] private bool has_collider = true;
    [SerializeField] private bool show_cards = true;

    [SerializeField] private float wide_x_distance = 1.5f;
    [SerializeField] private float x_distance = 1.5f;
    [SerializeField] private float z_distance = 0.02f;
    public Action<Visualizer, float> change_cards_on_hand_alpha;
    private Vector3 original_position;
    [SerializeField] private Vector3 original_collider_size;
    [SerializeField] private Vector3 focus_collider_size;
    [SerializeField] private GameObject sort_point;
    private List<GameObject> cards = null;


   void Start()
   {
       cards = new List<GameObject>();
       original_position = transform.position;
   }

   public void RemoveCard(GameObject card)
   {
       if (cards.Contains(card))
       {
            if (GameManager.GetInstance().LOG)
            {
                int card_transformed_index = cards.IndexOf(card) + Constants.LOG__PLAY_CARD_FROM_HAND_OFFSET;
                card.GetComponent<Card>().Log_CardPosition = card_transformed_index;
            }

            cards.Remove(card);
            change_cards_on_hand_alpha -= card.GetComponent<Visualizer>().ChangeInfoAlpha;

            SortCard();
       }
   }

   public void AddCard(GameObject card, Player owner, bool retrieving_from_board = false)
   {
        if (card == null)
        {
            if (!retrieving_from_board)
            {
                owner.ApplyPureDamage(Constants.PLAYER__PURE_DAMAGE_ON_EMPTY_DECK);
            }
            return;
        }

        bool non_graphical = GameManager.GetInstance().NON_GRAPHICAL;

        if (!retrieving_from_board)
        {
            GameObject on_game_card = Instantiate(card, owner.GetDeck().transform.position, Quaternion.Euler(0, 90, -90), transform);

            cards.Add(on_game_card);

            Card c = on_game_card.GetComponent<Card>();

            c.SetOwner(owner);
            SortCard(c);

            change_cards_on_hand_alpha += c.GetComponent<Visualizer>().ChangeInfoAlpha;
            c.GetComponent<Visualizer>().ToggleFront(non_graphical ? true : show_cards);
        }
        else
        {
            cards.Add(card);
            card.transform.parent = transform;
            SortCard();
            card.GetComponent<Visualizer>().ToggleFront(non_graphical ? true : show_cards);
        }
    }

   public bool IsFull()
   {
        return cards.Count == Constants.PLAYER__MAXIMUM_CARDS_ON_HAND;
   }

   public Vector3 GetSortPoint() => sort_point.transform.position;

   public void SortCard(Card new_card = null)
   {
        int cards_count = cards.Count;
        int i = -cards_count / 2;

        float show_time = 0.0f;
        float turn_time = 0.05f;
        float move_time = 0.15f;

        if (new_card != null)
        {
            show_time = 0.75f;
            turn_time = 0.33f;
            move_time = 0.33f;
        }

        foreach (GameObject card in cards)
        {
            card.GetComponent<Visualizer>().MoveToHand(transform.position + x_distance * i++ * Vector3.right, sort_point.transform.position, show_time, turn_time, move_time);    
        }

        TemporarilyDisableCollider(show_time + turn_time + move_time);
   }

   public void SortCardWide()
   {
        Vector3 current_pos = Vector3.zero;
        Vector3 base_position = new Vector3(0f, transform.position.y, -0.1f);
        if (base_position.y > 0)
        {
            current_pos = new Vector3(-cards.Count + 2f, 0, 0);
            base_position -= Vector3.up * 3f;
        }
        else
        {
            current_pos = new Vector3(-cards.Count - 3f, 0, 0);
            base_position += Vector3.up * 3f;
        }

        int i = 0;
        foreach (GameObject card in cards)
        {
            card.GetComponent<Visualizer>().Move(base_position + current_pos + wide_x_distance * i++ * Vector3.right, 0.1f);    
        }
   }

    public void OnMouseFocus()
    {
        if (!is_focusing)
        {
            SortCardWide();

            BoxCollider collider = GetComponent<BoxCollider>();
            collider.size = focus_collider_size;
            if (original_position.y < 0)
                collider.center = new Vector3(-original_position.x, 2f, 0);
            else
                collider.center = new Vector3(original_position.x, 2f, 0);


            is_focusing = true;
        }
        else
        {
            SortCard();

            BoxCollider collider = GetComponent<BoxCollider>();
            collider.size = original_collider_size;
            collider.center = Vector3.zero;

            is_focusing = false;
        }        
    }

    private void TemporarilyDisableCollider(float time)
    {
        if (has_collider)
            StartCoroutine(DisableCollider());

        IEnumerator DisableCollider()
        {
            SwitchOff();

            yield return new WaitForSeconds(time);

            SwitchOn();
        }
    }

    public void SwitchOff()
    {
        gameObject.GetComponent<Collider>().enabled = false;
    }

    public void SwitchOn()
    {
        gameObject.GetComponent<Collider>().enabled = true;
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    public CardData[] GetCardDatas()
    {
        CardData[] data = new CardData[cards.Count];
        for (int i = 0; i < data.Length; i++)
        {
            data[i] = cards[i].GetComponent<ICardData>().RetrieveCardData();
        }

        return data;
    }

    public int[] GetCardAsIds()
    {
        int[] card_ids = new int[cards.Count];
        for (int i = 0; i < card_ids.Length; i++)
        {
            card_ids[i] = APIDirector.GetInstance().GetCardID(cards[i].GetComponent<Card>().GetCardName());
        }

        return card_ids;
    }

    //---------API CALL SUBSECTION---------
    public void Play(int position, GameObject target)
    {
        if (cards.Count > position)
        {
            if (target != null)
            {
                cards[position].GetComponent<IActivateWithTarget>().Activate(true, target);
                return;
            }

            cards[position].GetComponent<Card>().Activate();
        }
    }

    public void Play(int position, GameObject minion1, GameObject minion2)
    {
        if (cards.Count > position)
        {
            if (minion1 != null && minion2 != null)
            {
                cards[position].GetComponent<TheFinalDuel>().Activate(true, minion1, minion2);
            }
        }
    }

}
