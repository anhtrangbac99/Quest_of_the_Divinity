using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APIDirector : MonoBehaviour
{
    private static APIDirector instance = null;
    private Dictionary<string, int> card_id_dict;
    private Client tcp_client;

    private SynchronizationContext sync_context = null;
    private TaskScheduler task_scheduler = null;
    private Queue<Action> tasks = null;

    private Dictionary<Action<int>, APIData> tasks_one_param = null;
    private Dictionary<Action<int, int>, APIData> tasks_two_param = null;
    private Dictionary<Action<int, int, bool>, APIData> tasks_three_param = null;

    private bool action_aborted = false;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            sync_context = SynchronizationContext.Current;
            task_scheduler = TaskScheduler.FromCurrentSynchronizationContext();

            tasks = new Queue<Action>();
            tasks_one_param = new Dictionary<Action<int>, APIData>();
            tasks_two_param = new Dictionary<Action<int, int>, APIData>();
            tasks_three_param = new Dictionary<Action<int, int, bool>, APIData>();
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        InitializeCardIDs();
        ConnectToServer();
    }

    public SynchronizationContext GetMainThread() => sync_context;

    /******************************************
     *                                        *
     *           API SETUP SECTION            *
     *                                        *
     ******************************************/
    private void InitializeCardIDs()
    {
        card_id_dict = new Dictionary<string, int>();

        // Open the csv file
        string local_file_path = Directory.GetCurrentDirectory() + @"\Assets\LocalData\card_ID.csv";
        using (StreamReader sr = new StreamReader(local_file_path))
        {
            sr.ReadLine();
            string data;
            while ((data = sr.ReadLine()) != null)
            {
                var splitted = data.Split(',');
                card_id_dict.Add(splitted[1], int.Parse(splitted[0]));
            }
        }
    }
    private void ConnectToServer()
    {
        tcp_client = new Client(Constants.API__HOST_NAME, Constants.API__HOST_PORT);
    }


    /******************************************
     *                                        *
     *       API COMMUNICATION SECTION        *
     *                                        *
     ******************************************/
    public void GetEnvState()
    {
        if (sync_context == SynchronizationContext.Current)
        {
            if (tcp_client.Tcp != null)
            {
                tcp_client.Tcp.SendEnvironmentState(GameManager.GetInstance().GetCurrentEnvironmentState());
            }
        }
        else
        {
            lock(tasks)
            {
                tasks.Enqueue(GetEnvState);
            }
        }
    }

    /// <summary>
    /// Command GameManager to play a card from the current turn player's hand.
    /// <para>Parameters:</para>
    /// <para><paramref name="position"/>: Desired card's position, valued from 0 to 8.</para>
    /// </summary>
    /// <param name="position"></param>
    public void PlayCardFromHand(int position)
    {
        if (sync_context == SynchronizationContext.Current)
        {
            GameManager.GetInstance().PlayerPlayCard(position);
            StartCoroutine(ActionCallback());
        }
        else
        {
            lock (tasks_one_param)
            {
                tasks_one_param.Add(PlayCardFromHand, new APIData(position));
            }
        }

    }


    /// <summary>
    /// Command GameManager to play a card from the current turn player's hand and activate its initial effect on a associative card, refer -1 to use on itself or discard the initial effect.
    /// <para>Parameters: </para>
    /// <para><paramref name="position"/>: Desired card's position, valued from 0 to 8</para>
    /// <para><paramref name="associative_position"/>: Associative card's position.</para>
    /// <para><paramref name="is_ally"/>: Indicates the <paramref name="associative_position"/> card belongs to player's board or opponent's board.</para>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="associative_position"></param>
    /// <param name="is_ally"></param>
    public void PlayCardFromHand(int position, int associative_position, bool is_ally)
    {
        if (sync_context == SynchronizationContext.Current)
        {
            GameManager.GetInstance().PlayerPlayCard(position, associative_position, is_ally);
            StartCoroutine(ActionCallback());
        }
        else
        { 
            lock (tasks_three_param)
            {
                tasks_three_param.Add(PlayCardFromHand, new APIData(position, associative_position, is_ally));
            }
        }
    }

    /// <summary>
    /// Command GameManager to let player's card at <paramref name="position"/> to attack enemy card at <paramref name="enemy_position"/>
    /// </summary>
    /// <param name="position"></param>
    /// <param name="enemy_position"></param>
    public void AttackCard(int position, int enemy_position)
    {
        if (sync_context == SynchronizationContext.Current)
        {
            GameManager.GetInstance().PlayerMinionAttack(position, enemy_position);
            StartCoroutine(ActionCallback());
        }
        else
        {
            lock (tasks_two_param)
            {
                tasks_two_param.Add(AttackCard, new APIData(position, enemy_position));
            }
        }
    }

    public void EndTurn()
    {
        if (sync_context == SynchronizationContext.Current)
        {
            GameManager.GetInstance().NotifyTurnOver();
            StartCoroutine(ActionCallback());
        }
        else
        {
            lock (tasks)
            {
                tasks.Enqueue(EndTurn);
            }
        }
    }

    public void NotifyGameOver()
    {
        if (sync_context == SynchronizationContext.Current)
        {
            tcp_client.Tcp.GameOver();
        }
        else
        {
            lock(tasks)
            {
                tasks.Enqueue(NotifyGameOver);
            }
        }
    }

    private IEnumerator ActionCallback()
    {
        yield return new WaitForSeconds(Constants.API__CALLBACK_WAIT);
        // To be sure that all the actions are done before the environment is passed to train client, wait two frames instead of 1
        while (!action_aborted)
        {
            if ((GameManager.GetInstance().VERSUS_AGENT && GameManager.GetInstance().GetCurrentPlayerID() == 1) 
                || (!GameManager.GetInstance().VERSUS_AGENT))
            {
                if (GameManager.GetInstance().NewEnvironmentAvailable())
                {
                    tcp_client.Tcp.ActionCallback();
                    yield break;
                }
            }

            yield return null;
        }

        if (action_aborted)
        {
            tcp_client.Tcp.AbortCallback();
            action_aborted = false;
        }
    }
    public int GetCardID(string card_name)
    {
        int card_id = (card_id_dict.ContainsKey(card_name) ? card_id_dict[card_name] : -1);
        if (card_id == -1)
        {
            Debug.Log(card_name);
        }

        return card_id;
    }
    public int GetTotalNumberOfCards() => card_id_dict.Count;

    public void AbortAction()
    {
        action_aborted = true;
    }

    public static APIDirector GetInstance() => instance;

    private void Update()
    {
        ExecuteTasks(ref tasks);
        ExecuteTasks(ref tasks_one_param);
        ExecuteTasks(ref tasks_two_param);
        ExecuteTasks(ref tasks_three_param);            
    }

    private void ExecuteTasks(ref Queue<Action> tasks)
    {
        while (tasks.Count > 0)
        {
            Action action = null;
            lock (tasks)
            {
                if (tasks.Count > 0)
                {
                    action = tasks.Dequeue();
                }
                action?.Invoke();
            }
        }

        tasks.Clear();
    }

    private void ExecuteTasks(ref Dictionary<Action<int>, APIData> tasks)
    {

        lock (tasks)
        {
            Action<int> action = null;
            foreach (KeyValuePair<Action<int>, APIData> _ in tasks)
            {
                action = _.Key;
                action?.Invoke(_.Value.first_param);
            }

            tasks.Clear();
        }
    }

    private void ExecuteTasks(ref Dictionary<Action<int, int>, APIData> tasks)
    {
        lock (tasks)
        {
            Action<int, int> action = null;
            foreach (KeyValuePair<Action<int, int>, APIData> _ in tasks)
            {
                action = _.Key;
                action?.Invoke(_.Value.first_param, _.Value.second_param);
            }

            tasks.Clear();
        }
    }

    private void ExecuteTasks(ref Dictionary<Action<int, int, bool>, APIData> tasks)
    {
        lock (tasks)
        {
            Action<int, int, bool> action = null;
            foreach (KeyValuePair<Action<int, int, bool>, APIData> _ in tasks)
            {
                action = _.Key;
                action?.Invoke(_.Value.first_param, _.Value.second_param, _.Value.third_param);
            }

            tasks.Clear();
        }
    }

    private void OnDestroy()
    {
        tcp_client.Close();
    }
}

public class APIData
{
    public int first_param;
    public int second_param;
    public bool third_param;

    public APIData(int first_param, int second_param = 0, bool third_param = false)
    {
        this.first_param = first_param;
        this.second_param = second_param;
        this.third_param = third_param;
    }
}

[Serializable]
public class JsonData
{
    public int player_id;
    public int player_life;
    public int player_gold;

    public int opponent_life;

    public float time;

    public JsonCard[] player_board_card_info;
    public JsonCard[] opponent_board_card_info;

    public int[] player_hand_card_id;
    public int[] player_remaining_cards_on_deck;
    public int[] opponent_remaining_cards;
    public int[] player_used_cards;
    public int[] opponent_used_cards;

    public JsonData(int player_id, int player_life, int player_gold, int opponent_life, float time, CardData[] player_board_card_info, CardData[] opponent_board_card_info, CardData[] player_hand_card_id, int[] player_remaining_cards_on_deck, int[] opponent_remaining_cards, int[] player_used_cards, int[] opponent_used_cards)
    {
        this.player_id = player_id;
        this.player_life = player_life;
        this.player_gold = player_gold;

        this.opponent_life = opponent_life;

        this.time = time;

        // Player card info section
        this.player_board_card_info = new JsonCard[Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD];

        for (int i = 0; i < Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD; i++)
        {
            if (i < player_board_card_info.Length)
            {
                this.player_board_card_info[i] = new JsonCard(player_board_card_info[i].id, 
                                                        player_board_card_info[i].status,
                                                        player_board_card_info[i].damage,
                                                        player_board_card_info[i].life);
            }
            else
            {
                // 0 for any places without a card
                this.player_board_card_info[i] = new JsonCard();
            }
        }

        // Opponent card info section
        this.opponent_board_card_info = new JsonCard[Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD];

        for (int i = 0; i < Constants.PLAYER__MAXIMUM_CARDS_ON_BOARD; i++)
        {
            if (i < opponent_board_card_info.Length)
            {
                this.opponent_board_card_info[i] = new JsonCard(opponent_board_card_info[i].id,
                                                            opponent_board_card_info[i].status,
                                                            opponent_board_card_info[i].damage,
                                                            opponent_board_card_info[i].life);
            }
            else
            {
                // 0 for any places without a card
                this.opponent_board_card_info[i] = new JsonCard();
            }
        }

        // Player hand cards id
        this.player_hand_card_id = new int[Constants.PLAYER__MAXIMUM_CARDS_ON_HAND];

        for (int i = 0; i < Constants.PLAYER__MAXIMUM_CARDS_ON_HAND; i++)
        {
            if (i < player_hand_card_id.Length)
            {
                this.player_hand_card_id[i] = player_hand_card_id[i].id;
            }
            else
            {
                // 0 for any places without a card
                this.player_hand_card_id[i] = 0;
            }
        }

        this.player_remaining_cards_on_deck = player_remaining_cards_on_deck;
        this.opponent_remaining_cards = opponent_remaining_cards;
        this.player_used_cards = player_used_cards;
        this.opponent_used_cards = opponent_used_cards;
    }
}

[Serializable]
public class JsonCard
{
    public int id;
    public int status;
    public int damage;
    public int life;

    public JsonCard()
    {
        id = 0;
    }
    public JsonCard(int id, int status = 0, int damage = 0, int life = 0)
    {
        this.id = id;
        this.status = status;
        this.damage = damage;
        this.life = life;
    }
}
