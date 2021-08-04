using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonVisualizer : MinionVisualizer
{
    [SerializeField] protected float fly_time = 0.5f;

    protected override IEnumerator AttackTarget(Vector3 target_position, float attack_time, TheFinalDuel duel_card = null, Minion target = null)
    {
        float elapsed_time = 0f;

        Vector3 original_position = transform.position;
        Vector3 fly_position = original_position + Vector3.back * 1.15f;
        Vector3 transform_up = transform.up;
        Vector3 target_direction = target_position - transform.position;

        GameManager.GetInstance().switch_off();
        while (elapsed_time <= fly_time)
        {
            transform.position = Vector3.Lerp(original_position, fly_position, elapsed_time / fly_time);
            transform.up = Vector3.Lerp(transform_up, target_direction, elapsed_time / fly_time);

            elapsed_time += Time.deltaTime;

            yield return null;
        }

        transform.position = fly_position;
        transform.up = target_direction;

        yield return new WaitForSeconds(0.33f);

        yield return AttackAnimation(fly_position, target_position, attack_time, duel_card != null);

        Vector3 current_position = transform.position;
        elapsed_time = 0f;
        while (elapsed_time <= fly_time)
        {
            transform.position = Vector3.Lerp(current_position, original_position, elapsed_time / fly_time);
            transform.up = Vector3.Lerp(target_direction, transform_up, elapsed_time / fly_time);

            elapsed_time += Time.deltaTime;

            yield return null;
        }

        transform.position = original_position;
        transform.up = transform_up;

        GameManager.GetInstance().switch_on();


        card.OnActivateCallback();

        if (duel_card != null)
        {
            card.FinalDuelCallback(duel_card, target);
        }
    }

    protected virtual IEnumerator AttackAnimation(Vector3 fly_position, Vector3 target_position, float attack_time, bool duel)
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
