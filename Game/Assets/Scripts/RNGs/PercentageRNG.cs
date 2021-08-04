using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PercentageRNG 
{
    public PercentageRNG() {}

    ///<summary> 
    /// Return true or false based on the percentage
    /// <para>
    /// Parameters:
    /// <para>
    /// <paramref name="percentage"/> Chance of playing true (percentage + 1).
    /// </para>
    ///</para>    
    ///</summary>
    public static bool Chance(int percentage)
    {
        if (percentage >= 100) return true;
        else if (percentage > 0)
        {
            int range = 100 - percentage;
            int chance = Random.Range(1, 101);

            if (chance >= range) return true;
        }

        return false;
    }
    
}
