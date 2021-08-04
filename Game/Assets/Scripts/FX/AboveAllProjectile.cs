using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboveAllProjectile : MonoBehaviour
{
    [SerializeField] private ParticleSystem fiery;
    [SerializeField] private ParticleSystem smoke;
    [SerializeField] private ParticleSystem explode;

    [SerializeField] private AnimationCurve curve;

    [SerializeField] private float destroy_delay_time = 1f;

    public void Throw(IDamagable target, Vector3 destination, int physical_damage, int spell_damage, float speed)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            target.OnReceiveDamage(null, physical_damage, spell_damage);
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
            Vector3 final_position = destination;

            transform.up = final_position - old_position;

            while (elapsed_time < time)
            {
                transform.position = Vector3.Lerp(old_position, final_position, curve.Evaluate(elapsed_time / time));
                elapsed_time += Time.deltaTime;

                yield return null;
            }

            fiery.Stop();
            smoke.Stop();
            explode.Play();

            target.OnReceiveDamage(null, physical_damage, spell_damage);

            yield return new WaitForSeconds(destroy_delay_time);

            Destroy(gameObject);

        }
    }
}
