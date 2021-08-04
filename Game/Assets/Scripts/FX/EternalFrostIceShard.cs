using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EternalFrostIceShard : MonoBehaviour
{
    [SerializeField] private GameObject shard;
    [SerializeField] private ParticleSystem chill;
    [SerializeField] private ParticleSystem blast;

    [SerializeField] private float destroy_delay_time = 0f;
    public void Throw(Minion target, Effect spawn_effect, float speed)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            target.AddEffect(spawn_effect);
            Destroy(gameObject);
        }
        else
        {
            StartCoroutine(Move());
        }

        IEnumerator Move()
        {
            float time = 1f / speed;
            float elapsed_time = 0f;
            Vector3 old_position = transform.position;
            Vector3 final_position = target.transform.position;

            transform.up = final_position - old_position;

            while (elapsed_time < time)
            {
                transform.position = Vector3.Lerp(old_position, final_position, elapsed_time / time);
                elapsed_time += Time.deltaTime;

                yield return null;
            }

            target.AddEffect(spawn_effect);

            shard.SetActive(false);
            chill.Stop();
            blast.Play();

            yield return new WaitForSeconds(destroy_delay_time);

            Destroy(gameObject);

        }
    }
}
