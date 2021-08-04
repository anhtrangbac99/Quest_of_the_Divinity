public enum GameState
{
    None, Minion_RequireTarget, Minion_ForceRequireTarget, Spell_RequireTarget, Sacrifice_RequireTarget, Sacrifice_RequireTargetBoth, Total
}
public enum MouseState
{
    Attack, Default
}
public enum EffectTargetType
{
    Minion, Player, Both, Total
}

public enum MinionClass
{
    None, Sage, Abyss, Warrior, Nature, Dragon, Total
}

public enum TargetSide
{
    Ally, Enemy,  Both_Sides
}

public enum TargetType
{
    Minion, Player, Minion_And_Player
}

public enum OutlineChangeType
{
    Attack, Ready, Defense, Off
}

public enum CardStatus
{
    Unavailable, Available
}

public enum UsedCardStatus
{
    Unused, Used
}
public enum APIHeuristic
{
    Highest_Damage, Highest_Life, Highest_Overall, Lowest_Damage, Lowest_Life, Lowest_Overall
}
public class Constants
{

    public const int PLAYER__MAXIMUM_CARDS_ON_HAND = 9;
    public const int PLAYER__MAXIMUM_CARDS_ON_BOARD = 7;
    public const int PLAYER__PURE_DAMAGE_ON_EMPTY_DECK = 3;

    public const int LOG__PLAY_CARD_FROM_HAND_OFFSET = 8;
    public const int LOG__CHOOSING_CARD_OFFSET = 1;
    public const int LOG__TARGET_OPPONENT_CARD_OFFSET = 1;
    public const int LOG__TARGET_PLAYER_ACTION_ID = 8;
    public const int LOG__TARGET_ALLY_CARD_OFFSET = 81;
    public const float LOG__WAIT_TIME = 0.5f;

    public const string API__HOST_NAME = "127.0.0.1";
    public const string SCENCE__MAIN = "Gameplay";
    public const int API__HOST_PORT = 27972;
    public const int API__FIELD_NUMBER = 4;
    public const int API__END_TURN_CODE = 999;
    public const int API__CLIENT_BUFFER_SIZE = 4096;
    public const float API__CALLBACK_WAIT = 1.25f;
}
