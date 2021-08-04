using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TOAAVisualizer : DragonVisualizer
{
    protected override IEnumerator AttackAnimation(Vector3 fly_position, Vector3 target_position, float attack_time, bool duel)
    {
        float elapsed_time = 0f;
        bool attacked = false;

        while (elapsed_time <= attack_time)
        {
            transform.position = Vector3.Lerp(fly_position, target_position, clip.Evaluate(elapsed_time / attack_time));

            if (!attacked && elapsed_time >= attack_time / 3.5f)
            {
                card.Strike(duel);

                attacked = true;
            }

            elapsed_time += Time.deltaTime;

            yield return null;
        }
    }
}
