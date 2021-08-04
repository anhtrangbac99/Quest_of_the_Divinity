using UnityEngine;

public interface IDamagable
{
    ///<summary> 
    /// Call once when this object dies
    /// </summary>
    void Death();

    ///<summary> 
    /// Manage the way this object get damaged and how it reacts to the source
    /// </summary>
    void OnReceiveDamage(IDamagable source, int physical_damage, int spell_damage);

    ///<summary> 
    /// Manage the way this object get damaged and how it reacts to the source
    /// </summary>
    void ScatterDebris(int damage, GameObject custom_debris = null);
}
