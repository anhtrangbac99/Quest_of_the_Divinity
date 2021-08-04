///<summary>
/// The Warchief subtype, it creates a special effect for itself, or its allies, or the enemies, or many composition from those 3.
///</summary>
public interface IWarchief
{
    ///<summary>
    /// Activate this minion's special effect (call this when the minion is activated, may be used on starting new turn).
    ///</summary>
   void ActivateWarchiefEffect();

   ///<summary>
    /// Deactivate this minion's special effect (call this when the minion dies, if needed).
    ///</summary>
   void DeactivateWarchiefEffect();
}
