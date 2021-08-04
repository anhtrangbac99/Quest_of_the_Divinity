using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : MonoBehaviour
{
    [SerializeField] private float destroy_delay_time = 0f;
    [SerializeField] private AnimationCurve curve;
    public void Throw(IDamagable target, int physical_damage, int spell_damage, float speed)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            target.OnReceiveDamage(null, physical_damage, spell_damage);

            Destroy(gameObject);
        }
        else
        {
            MonoBehaviour destination = target as MonoBehaviour;

            StartCoroutine(Move());

            IEnumerator Move()
            {
                float time = 1f / speed;
                float elapsed_time = 0f;
                Vector3 old_position = transform.position;
                Vector3 final_position = destination.transform.position;

                transform.up = final_position - old_position;

                while (elapsed_time < time)
                {
                    transform.position = Vector3.Lerp(old_position, final_position, curve.Evaluate(elapsed_time / time));
                    elapsed_time += Time.deltaTime;

                    yield return null;
                }

                target.OnReceiveDamage(null, physical_damage, spell_damage);

                ParticleSystem p = GetComponent<ParticleSystem>();
                if (p != null)
                {
                    p.Stop();
                }

                yield return new WaitForSeconds(destroy_delay_time);

                Destroy(gameObject);
            }
        }
    }
}
