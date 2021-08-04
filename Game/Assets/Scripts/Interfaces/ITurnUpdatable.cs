public interface ITurnUpdatable 
{
    ///<summary> 
    /// Call when this object is activated, or used
    /// </summary>
    void RegisterTurnUpdatable();
    ///<summary> 
    /// Call when this object is deactivated, or destroyed
    /// </summary>
    void UnregisterTurnUpdatable();
    ///<summary> 
    /// Called once every turn update
    /// </summary>
    void OnNewTurnUpdate();
}
