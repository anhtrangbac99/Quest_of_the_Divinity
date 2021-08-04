///<summary>
/// The champion subtype, minion of this subtype creates a special effect on its first turn
///</summary>
public interface IChampion
{
    ///<summary>
    /// Activate this minion's special effect (call this after activating minion card).
    ///</summary>
    void ActivateChampionEffect();

    ///<summary>
    /// Deactivate this minion's special effect (call this whn the first turn flag is false)
    ///</summary>
    void DeactivateChampionEffect();
}
