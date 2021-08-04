using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PercentageStackRNG
{
    public PercentageStackRNG() {}

    ///<summary> 
    /// Return a number from 0 - (n - 1), where n is the maximum number of probabilities. 
    /// <para>
    /// Return -1 if the array has no elements, or there's any error in computing the stacking probability.
    /// </para>
    /// <para>
    /// Parameters:
    /// <para>
    /// <paramref name="percentage_stack"/> Array of discrete percentage for each output.
    /// </para>
    ///</para>    
    ///</summary>
    public static int Chance(float[] percentage_stack)
    {
        if (percentage_stack.Length < 1) return -1;

        int chance = Random.Range(0, 100);
        
        float stack_percentage = 0;
        for (int i = 0; i < percentage_stack.Length; i++)
        {
            stack_percentage += percentage_stack[i];

            if (chance <= stack_percentage) return i;
        }

        return -1;
    }

    ///<summary> 
    /// Return a number from 0 - (n - 1), where n is the maximum number of probabilities. 
    /// <para>
    /// Return -1 if the array has no elements, or there's any error in computing the stacking probability.
    /// </para>
    /// <para>
    /// Parameters:
    /// <para>
    /// <paramref name="percentage_stack"/> Array of discrete percentage for each output.
    /// </para>
    ///</para>    
    ///</summary>
    public static int Chance(int[] percentage_stack)
    {
        if (percentage_stack.Length < 1) return -1;

        int chance = Random.Range(0, 100);
        
        int stack_percentage = 0;
        for (int i = 0; i < percentage_stack.Length; i++)
        {
            stack_percentage += percentage_stack[i];

            if (chance <= stack_percentage) return i;
        }

        return -1;
    }
}
