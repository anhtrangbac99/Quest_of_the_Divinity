using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool NON_GRAPHICAL = false;
    public bool LOG = false;
    public bool VERSUS_AGENT = false;
    private int current_turn = 0;
    private int both_side_chosen_card = 0;

    private static GameManager instance = null;

    [SerializeField] private GameObject api_director = null;

    private List<ITurnUpdatable> turn_updatables = null;
    private List<Player> players = null;
    [SerializeField] private Sprite guard_front = null;
    [SerializeField] private Sprite minion_front = null;

    [SerializeField] private Sprite[] damage_indicators;
    private GameState current_game_state = GameState.None;
    private Card requiring_target_card = null;
    private Timer timer = null;

    public Action spell_using_notifier;
    public Action switch_on;
    public Action switch_off;
    public Action<Player> minion_dies_notifier;
    [SerializeField] private Text win_text;

    private LogData current_log = null;

    private bool card_requiring_ally = false;
    private bool getting_new_log = false;
    private bool resetting = false;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    public void OnNewGameSetup()
    {
        turn_updatables = new List<ITurnUpdatable>();
        players = new List<Player>();

        switch_on = null;
        switch_off = null;

        // Set the flag to indicate should all the graphics be working
        if (APIDirector.GetInstance() == null)
        {
            Instantiate(api_director);
        }
        StartCoroutine(AutomaticallyStart(0.5f));

        current_game_state = GameState.None;

        if (win_text != null)
        {
            win_text.text = "";
        }

        IEnumerator AutomaticallyStart(float waitTime)
        {
            yield return new WaitForSeconds(waitTime);

            ChoosePlayerToMoveFirst();

            if (LOG)
            {
                LogWriter.Instance.SetLogName();
                current_log = new LogData(GetCurrentEnvironmentState());
            }

            if (/*NON_GRAPHICAL*/ true)
            {
                yield return new WaitForSeconds(waitTime * 2f);
                APIDirector.GetInstance().GetEnvState();
            }

            timer.started = true;
            timer.GetComponent<ITurnUpdatable>().OnNewTurnUpdate();

            Mouse.GetInstance().ResetHand();
        }
    }

    public void ChoosePlayerToMoveFirst()
    {
        current_turn = (PercentageRNG.Chance(49) ? 0 : 1);
        players[(current_turn + 1) % 2].RetrieveCard(3, true);

        players[current_turn].RetrieveCard(2, true);
        players[current_turn].OnNewTurnUpdate();
    }
    
    public GameState GetCurrentGameState() => current_game_state;
    
    public Player GetCurrentPlayerTurn() => players[current_turn];
    public int GetCurrentPlayerID() => current_turn;

    public Sprite GetGuardFront() => guard_front;
    public Sprite GetMinionFront() => minion_front;

    public Sprite GetStarBurstByDamageType(int type)
    {
        if (type < 1 || type > 3) return damage_indicators[0];

        return damage_indicators[type - 1];
    }
    /************************
    *                       *
    *    REGISTER CENTER    *
    *                       *
    *************************/
    public void RegisterPlayer(Player player)
    {
        if (players == null)
        {
            OnNewGameSetup();
        }
        players.Add(player);
    }

    public void RegisterTimer(Timer timer)
    {
        this.timer = timer;
    }

    /****************************
    *                           *
    *    NOTIFICATION CENTER    *
    *                           *
    *****************************/
    public void NotifyPlayerDeath(Player player)
    {
        if (win_text != null)
        {
            win_text.text = "Player " + ((players.IndexOf(player) + 1) % 2).ToString() + " Won!";
        }

        if (/*NON_GRAPHICAL*/ true)
        {
            APIDirector.GetInstance().NotifyGameOver();
            ResetGame();
        }
    }
    public void NotifyTurnOver()
    {
        players[current_turn].UpdateOnBoardOutline();
        
        current_turn = ++current_turn % 2;
        players[current_turn].GetComponent<ITurnUpdatable>().OnNewTurnUpdate();
        /******/
        if (!timer.started) timer.started = true;
        timer.OnNewTurnUpdate();

        if (LOG)
        {
            LogEndTurn();
        }
        NotifyAttackIsOver(true);
    }

    public void NotifySpellRequireTarget(Card spell_card, Player owner)
    {
        current_game_state = GameState.Spell_RequireTarget;
        requiring_target_card = spell_card;
    }

    public void NotifySacrificeRequireTarget(Card sacrifice_card, Player owner)
    {
        current_game_state = GameState.Sacrifice_RequireTarget;
        requiring_target_card = sacrifice_card;
    }
    public void NotifySacrificeRequireTargetBoth(Card sacrifice_card, Player owner)
    {
        current_game_state = GameState.Sacrifice_RequireTargetBoth;
        requiring_target_card = sacrifice_card;

        both_side_chosen_card = 0;
    }

    public void NotifyCardIsClicked(Card card, Player card_owner)
    {
        switch (current_game_state)
        {
            case GameState.None:
                if (card_owner == players[current_turn])
                {
                    Minion card_as_minion = card as Minion;
                    if (current_game_state == GameState.None && card_as_minion.GetWaitTurns() <= 0)
                    {
                        current_game_state = GameState. Minion_RequireTarget;
                        requiring_target_card = card;
                        Mouse.GetInstance().ChangeState(MouseState.Attack); 
                    }
                }
                break;
            case GameState.Minion_ForceRequireTarget:
                if ((card_owner != players[current_turn]) && !card_requiring_ally)
                {
                    requiring_target_card.SetLogTargetPosition(GetOpponentsMinions(requiring_target_card.GetOwner()).IndexOf(card) + Constants.LOG__TARGET_OPPONENT_CARD_OFFSET);

                    requiring_target_card.AssignTarget(card.GetComponent<IDamagable>());
                }
                else if (card_owner == players[current_turn] && card_requiring_ally)
                {
                    requiring_target_card.SetLogTargetPosition(requiring_target_card.GetOwner().GetMinionsOnBoard().IndexOf(card as Minion) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);

                    requiring_target_card.AssignTarget(card as Minion);
                }
                break;
            case GameState.Minion_RequireTarget:
                if (card_owner != players[current_turn])
                {
                    int opponent = (current_turn + 1) % 2;
                    List<Card> opponents_guards = players[opponent].GetGuards();
                    // If the opponent has guards, the current turn's player has to attack them first.
                    if (opponents_guards.Count > 0 && !opponents_guards.Contains(card))
                    {
                        NotifyAttackIsOver(true);
                    }
                    else
                    {
                        requiring_target_card.SetLogTargetPosition(GetOpponentsMinions(requiring_target_card.GetOwner()).IndexOf(card) + Constants.LOG__TARGET_OPPONENT_CARD_OFFSET);

                        requiring_target_card.AssignTarget(card.GetComponent<IDamagable>());
                    }
                }               

                break;
            case GameState.Spell_RequireTarget:
                Spell spell_card = requiring_target_card as Spell;
                if (spell_card.GetSpellCardTargetType() == TargetType.Minion)
                {
                    TargetSide side = spell_card.GetSpellCardTargetSide();
                    if (side == TargetSide.Enemy && players[current_turn] != card_owner)
                    {
                        spell_card.SetLogTargetPosition(GetOpponentsMinions(spell_card.GetOwner()).IndexOf(card) + Constants.LOG__TARGET_OPPONENT_CARD_OFFSET);

                        spell_card.AssignTarget(card);
                    }
                    else if (side == TargetSide.Ally && players[current_turn] == card_owner)
                    {
                        spell_card.SetLogTargetPosition(spell_card.GetOwner().GetCardsOnBoard().IndexOf(card) + Constants.LOG__TARGET_OPPONENT_CARD_OFFSET);

                        spell_card.AssignTarget(card);
                    }
                }
                break;
            case GameState.Sacrifice_RequireTarget:
                Sacrifice sacrifice_card = requiring_target_card as Sacrifice;
                if (sacrifice_card.GetSacrificeTargetType() == TargetType.Minion 
                    && card_owner == players[current_turn]
                    && sacrifice_card.GetMinimumMinionCost() <= card.GetCost())
                {
                    sacrifice_card.SetLogTargetPosition(GetOpponentsMinions(sacrifice_card.GetOwner()).IndexOf(card) + Constants.LOG__TARGET_ALLY_CARD_OFFSET);
                    
                    sacrifice_card.AssignTarget(card);
                }
                break;
            case GameState.Sacrifice_RequireTargetBoth:
                Sacrifice sac_card = requiring_target_card as Sacrifice;
                switch (both_side_chosen_card)
                {
                    case 0:
                        if (card_owner == players[current_turn])
                        {
                            both_side_chosen_card = 1;
                        }
                        else
                        {
                            both_side_chosen_card = 2;
                        }
                        sac_card.AssignTarget(card);
                        break;
                    case 1:
                        if (card_owner == GetOpponent(players[current_turn]))
                        {
                            sac_card.AssignTarget(card);
                            both_side_chosen_card = 3;
                        }
                        break;
                    case 2:
                        if (card_owner == players[current_turn])
                        {
                            sac_card.AssignTarget(card);
                            both_side_chosen_card = 3;
                        }
                        break;
                }
                break;
        }        
    }

    public void NotifyCardForceRequireTarget(Card card, bool ally = false)
    {
        card_requiring_ally = ally;
        current_game_state = GameState.Minion_ForceRequireTarget;
        requiring_target_card = card;

    }
    public void NotifyAvatarIsClicked(Player player)
    {
        if (current_game_state == GameState.Minion_RequireTarget)
        {
            // If they chose to attack the opponent's avater, and there's no guard, then let's go
            if (player != players[current_turn] && player.GetGuards().Count == 0)
            {
                requiring_target_card.SetLogTargetPosition(Constants.LOG__TARGET_PLAYER_ACTION_ID);

                requiring_target_card.AssignTarget(player.GetComponent<IDamagable>());
            }
            else
            {
                NotifyAttackIsOver(true);
            }
        }
        else if (current_game_state == GameState.Spell_RequireTarget)
        {
            // TODO
        }
    }

    public void NotifyAttackIsOver(bool target_aborted = false)
    {
        if (target_aborted)
        {
            switch (current_game_state)
            {
                case GameState.Minion_ForceRequireTarget:
                case GameState.Sacrifice_RequireTarget:
                case GameState.Sacrifice_RequireTargetBoth:
                case GameState.Spell_RequireTarget:
                    requiring_target_card.Deactivate();
                    break;
                case GameState.Minion_RequireTarget:
                    requiring_target_card.GetComponent<MinionVisualizer>().OnAbort();
                    break;
            }
        }
        else if (current_game_state == GameState.Spell_RequireTarget)
        {
            if (spell_using_notifier != null)
            {
                spell_using_notifier();
            }
        }

        Mouse.GetInstance().ChangeState(MouseState.Default); 

        current_game_state = GameState.None;

        requiring_target_card = null;
    }


    /************************************
    *                                   *
    *       IUPDATABLE HANDLER          *
    *                                   *
    ************************************/

    public void RegisterTurnUpdatable(ITurnUpdatable obj)
    {
        /*****/
        if (turn_updatables == null)
        {
            OnNewGameSetup();
        }
        turn_updatables.Add(obj);
    }

    public void UnregisterTurnUpdatable(ITurnUpdatable obj)
    {
        if (turn_updatables.Contains(obj))
        {
            turn_updatables.Remove(obj);
        }
        else
        {
            Debug.LogError("The object that you're trying to remove: " + obj.ToString() + " is not exist in the list anymore.");
        }

    }

    /*****************************
    *                            *
    *    RUNTIME DATA RETRIEVAL  *
    *                            *
    ******************************/

    public List<Card> GetOpponentsMinions(Player current_player)=> players[(players.IndexOf(current_player) + 1) % 2].GetCardsOnBoard();

    public BoardSide GetOpponentBoardSide(Player current_player) => players[(players.IndexOf(current_player) + 1) % 2].GetBoardSide();

    public Player GetOpponent(Player current_player) => players[(players.IndexOf(current_player) + 1) % 2];

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/

    public void ResetGame()
    {
        if (resetting)
            return;

        StartCoroutine(ResetGameAsync());

        IEnumerator ResetGameAsync()
        {
            resetting = true;
            OnNewGameSetup();

            AsyncOperation async_load = SceneManager.LoadSceneAsync(Constants.SCENCE__MAIN);
            while (!async_load.isDone)
            {
                yield return null;
            }

            resetting = false;
        }
    }
    public void RetrieveCurrentPlayerInformation(out int life, out int current_gold, out CardData[] on_board_cards, out CardData[] on_hand_cards, out int[] deck_remaining_cards, out int[] used_cards)
    {
        players[current_turn].RetrievePlayerInformation(out life, out current_gold, out on_board_cards, out on_hand_cards, out deck_remaining_cards, out used_cards);
    }

    public void RetrieveCurrentOpponentInformation(out int life, out CardData[] on_board_cards, out int[] remaining_cards, out int[] used_cards)
    {
        Player current_opponent = players[(current_turn + 1) % 2];

        life = current_opponent.GetPlayerLife();
        on_board_cards = current_opponent.GetPlayerOnBoardCardDatas();
        remaining_cards = current_opponent.GetOpponentRemainingCards();
        used_cards = current_opponent.GetUsedCards();
    }

    public float GetRemainingTime() => timer.GetRemainingTime();

    //---------API CALL SUBSECTION---------

    /// <summary>
    /// Command current turn player to play a card at desired position.
    /// </summary>
    /// <param name="position"></param>
    public void PlayerPlayCard(int position)
    {
        if (position < 0 || position > Constants.PLAYER__MAXIMUM_CARDS_ON_HAND - 1)
        {
            APIDirector.GetInstance().AbortAction();
            Debug.Log($"Unexpected position acquired: {position}");
        }
        else
        { 
            players[current_turn].Play(position);
        }
    }

    /// <summary>
    /// Command current turn player to play a card at desired position, and take its initial effect on the associative position.
    /// <para>Parameters:</para>
    /// <para><paramref name="position"/>: Card's position to play.</para>
    /// <para><paramref name="associative_position"/>: Associative card's position to take effect on. Use -1 to indicate that this card should take itself or none to take effect.</para>
    /// <para><paramref name="is_ally"/>: Choosing an ally card or enemy card.</para>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="associative_position"></param>
    /// <param name="is_ally"></param>
    public void PlayerPlayCard(int position, int associative_position, bool is_ally)
    {
        if (position < 0 || position > Constants.PLAYER__MAXIMUM_CARDS_ON_HAND || associative_position > Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD)
        {
            APIDirector.GetInstance().AbortAction();
            Debug.Log($"There is an unexpected value: {position} | {associative_position}");
            return;
        }

        Player current_turn_player = players[current_turn];
        if (associative_position == -1)
        {
            current_turn_player.Play(position);
        }
        else
        {
            if (associative_position >= 10 && associative_position <= 80)
            {
                current_turn_player.Play(position, associative_position);
            }
            else
            {
                BoardSide desired_board = is_ally ? current_turn_player.GetBoardSide() : GetOpponentBoardSide(current_turn_player); ;

                //current_turn_player.GetCardsOnBoard()[]
                current_turn_player.Play(position, desired_board.GetCardObject(associative_position));
            }            
        }
    }

    /// <summary>
    /// Command current turn player's card at <paramref name="position"/> to attack enemy at <paramref name="enemy_position"/>.
    /// <para>Parameters: </para>
    /// <para><paramref name="position"/>: current turn player's card position.</para>
    /// <para><paramref name="enemy_position"/>: desired attack position, use Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD to attack player.</para>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="enemy_position"></param>
    public void PlayerMinionAttack(int position, int enemy_position)
    {
        if (position >= 0 
            && position < Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD
            && enemy_position >= 0 
            && enemy_position <= Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD)
        {
            IDamagable target = enemy_position == Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD ? GetOpponent(players[current_turn]) as IDamagable : GetOpponentBoardSide(players[current_turn]).GetTarget(enemy_position);

            if (target == null)
            {
                Debug.Log($"Unexpected enemy_position encountered: {enemy_position}");
                APIDirector.GetInstance().AbortAction();
                return;
            }

            try
            {
                players[current_turn].GetCardsOnBoard()[position].SetLogTargetPosition(enemy_position + Constants.LOG__TARGET_OPPONENT_CARD_OFFSET);
                players[current_turn].CardAttack(position, target);
            }
            catch (Exception e)
            {
                Debug.Log($"P {position}, EP {enemy_position}, current player {current_turn}, num cards on board: {players[current_turn].GetCardsOnBoard().Count}. Error encountered: {e}");
            }
        }
        else
        {
            Debug.Log($"There is an unexpected value: {position} | {enemy_position}");
            APIDirector.GetInstance().AbortAction();
        }
    }

    //-------------LOG CALL SUBSECTION---------------

    public void LogPlayCard(int position)
    {
        LogPlayCardWithTarget(position, 0);
    }

    public void LogPlayCardWithTarget(int position, int associated_position)
    {
        current_log.SetTakenAction($"{position} {associated_position}");

        LogWriter.Instance.Write(JsonUtility.ToJson(current_log));

        StartCoroutine(GetNewLog());
    }
    public void LogPlayCardWithTarget(int position, string custom_position)
    {
        current_log.SetTakenAction($"{position} {custom_position}");

        LogWriter.Instance.Write(JsonUtility.ToJson(current_log));

        StartCoroutine(GetNewLog());
    }
    public void LogCardAttack(int position, int associated_position)
    {
        current_log.SetTakenAction($"{position} {associated_position}");

        LogWriter.Instance.Write(JsonUtility.ToJson(current_log));

        StartCoroutine(GetNewLog());
    }

    public void LogEndTurn()
    {
        current_log.SetTakenAction(Constants.API__END_TURN_CODE.ToString());

        LogWriter.Instance.Write(JsonUtility.ToJson(current_log));

        StartCoroutine(GetNewLog());
    }
    private IEnumerator GetNewLog()
    {
        getting_new_log = true;

        yield return new WaitForSeconds(Constants.LOG__WAIT_TIME);

        current_log = new LogData(GetCurrentEnvironmentState());
        getting_new_log = false;
    }

    public bool NewEnvironmentAvailable() => !getting_new_log;

    public string GetCurrentEnvironmentState()
    {
        int current_player_id = GetCurrentPlayerID();

        int current_player_life, current_player_gold, current_opponent_life;
        float remaining_time;

        CardData[] current_player_on_board_data;
        CardData[] current_player_on_hand_data;
        CardData[] current_opponent_on_board_data;

        int[] current_player_remaining_cards_on_deck;
        int[] current_opponent_remaining_cards;
        int[] current_player_used_cards;
        int[] current_opponent_used_cards;

        RetrieveCurrentPlayerInformation(out current_player_life, out current_player_gold, out current_player_on_board_data, out current_player_on_hand_data, out current_player_remaining_cards_on_deck, out current_player_used_cards);

        RetrieveCurrentOpponentInformation(out current_opponent_life, out current_opponent_on_board_data, out current_opponent_remaining_cards, out current_opponent_used_cards);

        remaining_time = GetRemainingTime();

        JsonData json = new JsonData(current_player_id, current_player_life, current_player_gold, current_opponent_life, remaining_time, current_player_on_board_data, current_opponent_on_board_data, current_player_on_hand_data, current_player_remaining_cards_on_deck, current_opponent_remaining_cards, current_player_used_cards, current_opponent_used_cards);
        
        return JsonUtility.ToJson(json);
    }
}
