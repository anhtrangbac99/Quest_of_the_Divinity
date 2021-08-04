using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Deck : MonoBehaviour
{
    [SerializeField] private GameObject[] one_gold_cards;
    [SerializeField] private GameObject[] two_gold_cards;

    [SerializeField] private GameObject[] three_gold_cards;
    [SerializeField] private GameObject[] four_gold_cards;
    [SerializeField] private GameObject[] five_gold_cards;
    [SerializeField] private GameObject[] six_gold_cards;
    [SerializeField] private GameObject[] seven_gold_cards;
    [SerializeField] private GameObject[] eight_gold_cards;

    private float[][] card_distribution;
    private int[] remaining_cards_on_deck;

    void Awake()
    {
        SetDistribution();
        /************************/
    }

    private void Start()
    {
        remaining_cards_on_deck = new int[APIDirector.GetInstance().GetTotalNumberOfCards()];
        for (int i = 0; i < remaining_cards_on_deck.Length; i++)
        {
            remaining_cards_on_deck[i] = (int)CardStatus.Available;
        }
    }

    ///<summary> 
    /// Retrieve a card from player's deck to player's hand. 
    /// <para>
    /// Parameters:
    /// <para>
    /// <paramref name="current_gold_limit"/> Player's current gold limit.
    /// </para>
    ///</para>    
    ///</summary>
    public GameObject RetrieveCard(int current_gold_limit)
    {
        if (current_gold_limit <= 0) return null;
        else
        {
            return ProvideCard(card_distribution[current_gold_limit >= 10 ? 9 : (current_gold_limit - 1)]);
        }
    }

    public GameObject RetrieveCard(MinionClass minion_class)
    {
        List<GameObject> all_minions = new [] {one_gold_cards, two_gold_cards, three_gold_cards, four_gold_cards, five_gold_cards, six_gold_cards, seven_gold_cards, eight_gold_cards }
        .SelectMany(x => x)
        .ToList();

        List<GameObject> desired_minions = new List<GameObject>();

        foreach (GameObject card in all_minions)
        {
            Minion minion = card.GetComponent<Minion>();
            if (minion != null && minion.GetClass() == minion_class)
            {
                desired_minions.Add(card);
            }
        }

        return (desired_minions.Count == 0 ? null : desired_minions[Random.Range(0, desired_minions.Count)]);
    }

    public int[] GetRemainingCardStatus() => remaining_cards_on_deck;
    public void SetRemainingCardStatus(int id, CardStatus status)
    {
        if (id > 0 && id <= remaining_cards_on_deck.Length)
        {
            remaining_cards_on_deck[id - 1] = (int)status;
        }
    }
    private MinionClass FindClass(GameObject x)
    {
        return x.GetComponent<Minion>().GetClass();
    }

    private GameObject ProvideCard(float[] distribution)
    {
        GameObject[] card_group;
        int group_number;

        if (one_gold_cards.Length + two_gold_cards.Length + three_gold_cards.Length + four_gold_cards.Length + five_gold_cards.Length + six_gold_cards.Length + seven_gold_cards.Length + eight_gold_cards.Length == 0)
        {
            return null;
        }

        /******************
        *                 *
        *   CRITICAL!     *
        *                 *
        *******************/
        do
        {
            group_number = PercentageStackRNG.Chance(distribution);
            switch(group_number)
            {
                case 0:
                    card_group = one_gold_cards;
                    break;
                case 1:
                    card_group = two_gold_cards;
                    break;
                case 2:
                    card_group = three_gold_cards;
                    break;
                case 3:
                    card_group = four_gold_cards;
                    break;
                case 4:
                    card_group = five_gold_cards;
                    break;
                case 5:
                    card_group = six_gold_cards;
                    break;
                case 6:
                    card_group = seven_gold_cards;
                    break;
                case 7:
                    card_group = eight_gold_cards;
                    break;
                default:
                    return null;
            }
        } while (card_group == null || card_group.Length < 1);
       
        GameObject selected_card = card_group[Random.Range(0, card_group.Length)];

        card_group = card_group.Where(x => x != selected_card).ToArray();

        // Save the new cards to the existing pointer.
        switch(group_number)
        {
            case 0:
                one_gold_cards = card_group;
                break;
            case 1:
                two_gold_cards = card_group;
                break;
            case 2:
                three_gold_cards = card_group;
                break;
            case 3:
                four_gold_cards = card_group;
                break;
            case 4:
                five_gold_cards = card_group;
                break;
            case 5:
                six_gold_cards = card_group;
                break;
            case 6:
                seven_gold_cards = card_group;
                break;
            case 7:
                eight_gold_cards = card_group;
                break;
        }

        // Remove this card's availability.
        int card_id = APIDirector.GetInstance().GetCardID(selected_card.GetComponent<Card>().GetCardName());
        SetRemainingCardStatus(card_id, CardStatus.Unavailable);

        return selected_card;
    }

    private void SetDistribution()
    {
        card_distribution = new float[10][];

        // 1-gold distribution
        card_distribution[0] = new float [8]{100f, 0f, 0f, 0f, 0f, 0f, 0f, 0f};

        // 2-gold distribution
        card_distribution[1] = new float [8]{80f, 20f, 0f, 0f, 0f, 0f, 0f, 0f};

        // 3-gold distribution
        card_distribution[2] = new float [8]{60f, 30f, 10f, 0f, 0f, 0f, 0f, 0f};

        // 4-gold distribution
        card_distribution[3] = new float [8]{40f, 30f, 20f, 10f, 0f, 0f, 0f, 0f};

        // 5-gold distribution
        card_distribution[4] = new float [8]{25f, 25f, 20f, 20f, 10f, 0f, 0f, 0f};

        // 6-gold distribution
        card_distribution[5] = new float [8]{10f, 20f, 25f, 25f, 15f, 5f, 0f, 0f};

        // 7-gold distribution
        card_distribution[6] = new float [8]{10f, 12f, 20f, 25f, 20f, 9f, 4f, 0f};

        // 8-gold distribution
        card_distribution[7] = new float [8]{8f, 10f, 15f, 25f, 20f, 12f, 8f, 2f};

        // 9-gold distribution
        card_distribution[8] = new float [8]{5f, 8f, 12f, 20f, 25f, 15f, 10f, 5f};

        // 10-gold distribution
        card_distribution[9] = new float [8]{5f, 8f, 12f, 18f, 25f, 15f, 10f, 7f};
    }
}
