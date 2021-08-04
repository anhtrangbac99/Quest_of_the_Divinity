using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : MonoBehaviour, IDamagable, ITurnUpdatable, IMouseDownHandler
{
    private int[] used_cards;
    [SerializeField] private int life = 40;
    [SerializeField] private int maximum_life = 40;
    private int physical_shield = 0;
    private int spell_shield = 0;

    public int Physical_shield 
    {
        get { return physical_shield; } 
        set 
        { 
            physical_shield = Mathf.Abs(value);
            shield_indicator.Indicate(physical_shield, spell_shield);
        } 
    }
    public int Spell_shield
    {
        get { return spell_shield; }
        set 
        { 
            spell_shield = Mathf.Abs(value); 
            shield_indicator.Indicate(physical_shield, spell_shield);
        }
    }
    [SerializeField] private int current_gold = 1;
    [SerializeField] private int current_maximum_gold = 1;
    [SerializeField] private int maximum_gold_limit = 10;
    private int placing_card_gold_modifier = 0;    
    [SerializeField] private Hand player_hand = null;
    [SerializeField] private Deck player_deck = null;
    [SerializeField] private BoardSide player_board_side = null;
    [SerializeField] protected DamageIndicator damage_indicator = null;
    [SerializeField] private GameObject minion_activation_effect = null;
    [SerializeField] private GameObject minion_dropped_effect = null;
    [SerializeField] private GameObject minion_death_effect = null;
    [SerializeField] private GameObject minion_damaged_effect = null;
    [SerializeField] private GameObject healing_effect = null;
    private Dictionary<string, Effect> minion_related_effects;
    private Dictionary<string, Effect> player_related_effects;

    [SerializeField] private List<Card> guards;

    [SerializeField] private LifeIndicator life_indicator = null;
    [SerializeField] private GoldIndicator gold_indicator = null;
    [SerializeField] private ShieldsIndicator shield_indicator = null;
    public Action<IDamagable, IDamagable> attack_notification;
    public Action<Player, Card> placing_card_notification;
    public Action<int> player_receive_damage_notifier;
    public Action<Minion, int> minion_receive_damage_notifier;

    private void Start()
    {
        GameManager.GetInstance().RegisterPlayer(this);
        
        minion_related_effects = new Dictionary<string, Effect>();
        player_related_effects = new Dictionary<string, Effect>();
        guards = new List<Card>();

        used_cards = new int[APIDirector.GetInstance().GetTotalNumberOfCards()];
        for (int i = 0; i < used_cards.Length; i++)
        {
            used_cards[i] = (int)UsedCardStatus.Unused;
        }
        /****************/
        UpdateLife();
    }

    public bool RetrieveCard(int number = 1, bool start_game = false)
    {
        int retrieved = 0;
        for (int i = 0; i < number && !player_hand.IsFull(); i++)
        {
            player_hand.AddCard(player_deck.RetrieveCard((start_game ? 3 : current_maximum_gold)), this);
            retrieved++;           
        }        

        return retrieved == number;
    }

    public bool RetrieveCard(MinionClass minion_class)
    {
        if (!player_hand.IsFull())
        {
            player_hand.AddCard(player_deck.RetrieveCard(minion_class), this);
            return true;
        }
        return false;
    }

    public bool PlaceThisCard(Card card, bool is_minion = true, bool using_gold = true)
    {
        if (is_minion && player_board_side.IsFull())
        {
            return false;
        }

        if (using_gold)
        {
            placing_card_notification?.Invoke(this, card);

            int card_cost = card.GetCost();
            int final_cost = card_cost - placing_card_gold_modifier;
            final_cost = (final_cost < 0 ? 0 : final_cost);
            
            if (current_gold >= final_cost)
            {
                current_gold -= final_cost;
                UpdateGold();

                placing_card_notification?.Invoke(null, null);
                placing_card_gold_modifier = 0;
            }
            else
            {
                return false;
            }
        }

        player_hand.RemoveCard(card.gameObject);

        return (is_minion ? player_board_side.PlaceCard(card.gameObject) : true);
    }
    public void RemoveThisCardFromBoard(Card card, bool sort = true)
    {
        player_board_side.RemoveCardFromBoard(card.gameObject, sort);
        // If this minion is a guard, remove it from the player's guard list
        Minion minion_card = card.GetComponent<Minion>();
        if (minion_card != null && minion_card.IsGuard())
        {
            RemoveGuard(card);
        }
    }

    public void AddCardToHand(Card card)
    {
        player_hand.AddCard(card.gameObject, this, true);
    }

    public void AddGold(int amount, bool exceed = false)
    {
        current_gold += amount;
        if (current_gold < 0) current_gold = 0;
        if (!exceed && current_gold > 10) current_gold = 10;

        UpdateGold();

    }
    
    // This function would cause a little bit struggle and/or confusing on cummulating gold modifier with different modify card numbers.
    public void ModifyGoldToPlaceCard(int gold)
    {
        placing_card_gold_modifier += gold;
    }
    public void ModifyCurrentMaximumGoldLimit(int amount, bool exceed = false)
    {
        current_maximum_gold += amount;
        if (!exceed && current_maximum_gold >= maximum_gold_limit) 
        {
            current_maximum_gold = maximum_gold_limit;
        }
        else if (current_maximum_gold < 0)
        {
            current_maximum_gold = 0;
        }
    }
    public void AddNewGuard(Card card)
    {
        if (!guards.Contains(card))
        {
            guards.Add(card);
        }
    }
    public void RemoveGuard(Card card)
    {
        if (guards.Contains(card))
        {
            guards.Remove(card);
        }
        else
        {
            Debug.LogError("The guard you are trying to remove: " + card.ToString() + " is no longer exist in the list!");
        }
    }

    public List<Card> GetGuards()
    {
        return guards;
    }

    public List<Card> GetCardsOnBoard()
    {
        List<GameObject> game_object_cards = player_board_side.GetCards();

        List<Card> cards = new List<Card>();
        foreach(GameObject g in game_object_cards)
        {
            cards.Add(g.GetComponent<Card>());
        }

        return cards;
    }

    public List<Minion> GetMinionsOnBoard(bool wounded_only = false)
    {
        List<GameObject> game_object_cards = player_board_side.GetCards();

        List<Minion> cards = new List<Minion>();
        foreach(GameObject g in game_object_cards)
        {
            Minion _ = g.GetComponent<Minion>();
            if (!wounded_only)
            {
                cards.Add(_);
            }
            else
            {
                if (_.IsWounded())
                {
                    cards.Add(_);
                }
            }
        }

        return cards;
    }

    public BoardSide GetBoardSide() => player_board_side;

    public Hand GetHand() => player_hand;
    public Deck GetDeck() => player_deck;
    public GameObject GetBasicActivationEffect() => minion_activation_effect;
    public GameObject GetBasicDropEffect() => minion_dropped_effect;
    public GameObject GetBasicDeathEffect() => minion_death_effect;
    public GameObject GetBasicDamagedEffect() => minion_damaged_effect;
    public bool CanPlaceCard() => !player_board_side.IsFull();
    public void Heal(int amount)
    {
        life += amount;
        if (life >= maximum_life)
        {
            life = maximum_life;
        }

        Instantiate(healing_effect, transform.position + Vector3.down, Quaternion.Euler(-90, 0, 0));

        UpdateLife();
    }

    public void ApplyPureDamage(int pure_damage)
    {
        this.life -= pure_damage;

        UpdateLife();
        if (this.life <= 0)
        {
            life = 0;
            UpdateLife();
            Death();
        }
    }
    private void UpdateLife()
    {
        life_indicator.Indicate(life, maximum_life);
    }

    private void UpdateGold()
    {
        gold_indicator?.Indicate(current_gold, current_maximum_gold);
    }

    public void ApplyAllEffect(Card card)
    {
        foreach(Effect effect in minion_related_effects.Values)
        {
            effect.TakeEffect(card);
        }
    }
    public void AddEffectForMinion(Effect effect)
    {
        string effect_id = effect.GetID();
        if (!minion_related_effects.ContainsKey(effect_id))
        {
            Effect object_effect = Instantiate(effect) as Effect;
            object_effect.AssignTarget(gameObject);
            minion_related_effects.Add(effect_id, object_effect);
        }
        else
        {
            minion_related_effects[effect_id].ResetRemainingTurn();
        }
    }

    public void RemoveEffectFromMinion(Effect effect)
    {
        string effect_id = effect.GetID();

        if (minion_related_effects.ContainsKey(effect_id))
        {
            minion_related_effects[effect_id].RevertEffect();
        }
    }

    public void AddEffectForPlayer(Effect effect, int num_turns = -1)
    {
        string effect_id = effect.GetID();

        if (!player_related_effects.ContainsKey(effect_id))
        {
            Effect object_effect = Instantiate(effect) as Effect;
            object_effect.AssignTarget(gameObject);

            if (num_turns > 0)
            {
                object_effect.ResetRemainingTurn(false, num_turns);
            }

            player_related_effects.Add(effect_id, object_effect);
        }
        else
        {
            player_related_effects[effect_id].ResetRemainingTurn((num_turns == -1), num_turns);
        }
    }

    /************************
    *                       *
    *       IDAMAGABLE      *
    *                       *
    *************************/
    public void Death()
    {
        GameManager.GetInstance().NotifyPlayerDeath(this);
    }

    public void OnReceiveDamage(IDamagable source, int physical_damage, int spell_damage)
    {
        physical_shield -= physical_damage;
        spell_shield -= spell_damage;

        life += (physical_shield < 0 ? physical_shield : 0);
        life += (spell_shield < 0 ? spell_shield : 0);
        
        player_receive_damage_notifier?.Invoke(physical_shield + spell_shield);

        // INDICATION SECTION
        // // Damage Indication
        int calc_damage = physical_damage + spell_damage;
        int damage_type = (physical_damage > 0 ? 1 : 0) + (spell_damage > 0 ? 2 : 0);
        damage_indicator.Indicate(calc_damage, damage_type);

        // // Shield Indication
        shield_indicator.Indicate(physical_shield, spell_shield);

        physical_shield = (physical_shield < 0 ? 0 : physical_shield);
        spell_shield = (spell_shield < 0 ? 0 : spell_shield);
        
        /*************/
        UpdateLife();
        if (life <= 0)
        {
            life = 0;
            UpdateLife();
            Death();
        }
    }

    public void ScatterDebris(int damage, GameObject custom_debris = null)
    { }

    /************************
    *                       *
    *     ITURNUPDATABLE    *
    *                       *
    *************************/

    public void RegisterTurnUpdatable()
    { }
    public void UnregisterTurnUpdatable()
    { }
    public void OnNewTurnUpdate()
    {
        current_maximum_gold++;
        if (current_maximum_gold >= maximum_gold_limit)
        {
            current_maximum_gold = maximum_gold_limit;
        }

        if (!RetrieveCard())
        {
            // DO SOMETHING WHEN HAND IS FULL.
        }

        player_board_side.UpdateCardTurn();

        foreach (Effect effect in minion_related_effects.Values)
        {
            if (effect.IsActivating())
            {
                effect.OnNewTurnUpdate();
            }
        }

        foreach (Effect effect in player_related_effects.Values)
        {
            if (effect.IsActivating())
            {
                effect.OnNewTurnUpdate();
            }
        }

        current_gold = current_maximum_gold;

        /*****/
        UpdateGold();
    }

    public void UpdateOnBoardOutline()
    {
        List<GameObject> minions = player_board_side.GetCards();
        foreach (GameObject _ in minions)
        {
            _.GetComponent<MinionVisualizer>().EvaluateOutline(OutlineChangeType.Off);
        }

        if (guards.Count > 0)
        {
            foreach(Card _ in guards)
            {
                _.GetComponent<MinionVisualizer>().EvaluateOutline(OutlineChangeType.Defense);
            }
        }
    }

    /*************************
    *                        *
    *    IMOUSEDOWNHANdLER   *
    *                        *
    **************************/

    public void OnMouseClickDown()
    {
        GameManager.GetInstance().NotifyAvatarIsClicked(this);
    }

    public void OnAbort() {}

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/

    public int GetPlayerLife() => life;
    public CardData[] GetPlayerOnBoardCardDatas() => player_board_side.GetCardDatas();
    public CardData[] GetPlayerOnHandCardDatas() => player_hand.GetCardDatas();

    public void UpdateCardStatus(int id, UsedCardStatus status)
    {
        if (id > 0 && id <= used_cards.Length)
        {
            used_cards[id - 1] = (int)status;
        }
    }
    public int[] GetUsedCards() => used_cards;
    public int[] GetRemainingCardsOnDeck() => player_deck.GetRemainingCardStatus();
    public int[] GetOpponentRemainingCards()
    {
        int card_list_length = APIDirector.GetInstance().GetTotalNumberOfCards();
        int[] _remaining = new int[card_list_length];
        int[] remaining_hand = player_hand.GetCardAsIds();

        Array.Copy(player_deck.GetRemainingCardStatus(), _remaining, card_list_length);
        for (int i = 0; i < remaining_hand.Length; i++)
        {
            _remaining[remaining_hand[i] - 1] = (int)CardStatus.Available;
        }

        return _remaining;
    }

    public void RetrievePlayerInformation(out int life, out int current_gold, out CardData[] on_board_cards, out CardData[] on_hand_cards, out int[] deck_remaining_cards, out int[] used_cards)
    {
        life = GetPlayerLife();
        current_gold = this.current_gold;
        on_board_cards = GetPlayerOnBoardCardDatas();
        on_hand_cards = GetPlayerOnHandCardDatas();
        deck_remaining_cards = GetRemainingCardsOnDeck();
        used_cards = GetUsedCards();
    }

    //---------API CALL SUBSECTION---------
    public void Play(int position, GameObject target = null) => player_hand.Play(position, target);
    public void Play(int position, int associative_number)
    {
        GameObject minion1 = player_board_side.GetCardObject(associative_number / 10 - 1);
        GameObject minion2 = GameManager.GetInstance().GetOpponentBoardSide(this).GetCardObject(associative_number % 10 - 1);

        player_hand.Play(position, minion1, minion2);
    }

    public void CardAttack(int position, IDamagable target) => player_board_side.CardAttack(position, target);
}
