public interface ICardData
{
    CardData RetrieveCardData();
}

public class CardData
{
    public bool is_minion;
    public int id;
    public int status;
    public int damage;
    // public int physical_damage;
    // public int spell_damage;
    public int life;

    public CardData(bool is_minion, int id, int status = 0, int damage = 0, int life = 0)
    {
        this.is_minion = is_minion;
        this.id = id;
        this.status = status;
        this.damage = damage;
        this.life = life;
    }
}
