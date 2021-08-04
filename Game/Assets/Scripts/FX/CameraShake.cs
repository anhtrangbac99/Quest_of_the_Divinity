using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float magnitude = 1f;
   
    public void Shake(float time, float time_interval)
    {
        StartCoroutine(RealShake());

        IEnumerator RealShake()
        {
            float elapsed_time = 0f;

            Vector3 original_position = transform.position;

            while (elapsed_time < time)
            {
                float percent_complete = elapsed_time / time;
                float damper = 1f - Mathf.Clamp(4f * percent_complete - 3f, 0f, 1f);

                float pos_x = Random.Range(-1f, 1f);
                float pos_y = Random.Range(-1f, 1f);

                pos_x *= magnitude * damper;
                pos_y *= magnitude * damper;

                transform.position = original_position + new Vector3(pos_x, pos_y, 0);

                elapsed_time += time_interval;

                yield return new WaitForSeconds(time_interval);
            }
        }
    }
}
