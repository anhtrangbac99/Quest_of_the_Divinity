using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingHeal : MonoBehaviour
{
    [SerializeField] private float width = 1f;
    [SerializeField] private float height = 1f;
    [SerializeField] private float speed = 1f;
    [SerializeField] private float destroy_delay_time = 0f;
    [SerializeField] private AnimationCurve curve;
    private float offset = 0f;
    public float Offset
    {
        get { return offset; }
        set { offset = value; }
    }

    private Coroutine spin = null;

    void Start()
    {
        spin = StartCoroutine(Spin());
    }
    public void Throw(Minion target, int amount, bool life_only, float speed)
    {
        StopCoroutine(spin);
        MonoBehaviour destination = target as MonoBehaviour;

        StartCoroutine(Move());

        IEnumerator Move()
        {
            float time = 1f / speed;
            float elapsed_time = 0f;
            Vector3 old_position = transform.position;
            Vector3 final_position = destination.transform.position;

            transform.up = final_position - old_position;

            GameManager.GetInstance().switch_off();

            while(elapsed_time < time)
            {
                transform.position = Vector3.Lerp(old_position, final_position, curve.Evaluate(elapsed_time / time));

                elapsed_time += Time.deltaTime;

                yield return null;
            }

            yield return StartCoroutine(Spin(true, 2 * destroy_delay_time / 3f));

            GameManager.GetInstance().switch_on();

            target.ModifyLife(amount, life_only);

            GetComponent<ParticleSystem>().Stop();
            Destroy(gameObject, destroy_delay_time / 3f);
        }
    }

    IEnumerator Spin(bool shrink = false, float shrink_time = 0f)
    {
        Vector3 original_position = transform.position;
        Vector3 current_scale = transform.localScale;
        float elapsed_time = 0f;

        while (true)
        {
            float x_coord = Mathf.Sin(elapsed_time * speed + offset) * width ;
            float y_coord = Mathf.Cos(elapsed_time * speed + offset) * height;

            Vector3 next_position = new Vector3(x_coord, y_coord, 0f);

            transform.position = original_position + next_position;

            if (shrink)
            {
                transform.localScale = Vector3.Lerp(current_scale, Vector3.zero, elapsed_time / shrink_time);
                if (elapsed_time + 0.1f >= shrink_time)
                {
                    GetComponent<ParticleSystem>().Stop();
                    yield break;
                }
            }

            elapsed_time += Time.deltaTime;

            yield return null;
        }        
    }
}
