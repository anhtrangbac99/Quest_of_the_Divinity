using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindfieldCommander : Minion, IChampion
{
    [SerializeField] private GameObject soldier;
    [SerializeField] private int num_soldier_to_place = 2;

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }

    public void ActivateChampionEffect()
    {
        StartCoroutine(CallToArms());

        IEnumerator CallToArms()
        {   
            GameManager.GetInstance().switch_off();

            float elapsed_time = 0f;
            float time = 1f;
            float time_interval = 0.05f;
            float magnitude = 2.5f;

            Quaternion original = transform.rotation;

            while (elapsed_time < time)
            {
                float percent_complete = elapsed_time / time;
                float damper = 1f - Mathf.Clamp(4f * percent_complete - 3f, 0f, 1f);

                float pos_x = Random.Range(-1f, 1f);
                float pos_y = Random.Range(-1f, 1f);

                pos_x *= magnitude * damper;
                pos_y *= magnitude * damper;

                transform.Rotate(pos_x, pos_y, 0);

                elapsed_time += time_interval;

                yield return new WaitForSeconds(time_interval);
            }

            transform.rotation = original;

            for (int i = 0; i < num_soldier_to_place && owner.CanPlaceCard(); i++)
            {
                GameObject the_soldier = Instantiate(soldier);
                Minion _ = the_soldier.GetComponent<Minion>();
                _.SetOwner(owner);
                _.Activate(false);
            }

            GameManager.GetInstance().switch_on();
        }
        
    }

    public void DeactivateChampionEffect()
    {

    }
}
