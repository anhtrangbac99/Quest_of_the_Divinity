using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attractor : MonoBehaviour
{
    [SerializeField] protected GameObject destination;

    public void Move(Vector3 destination, float live_time)
    {
        this.destination.transform.position = destination;
        StartCoroutine(WaitAndDie());

        IEnumerator WaitAndDie()
        {
            yield return new WaitForSeconds(live_time / 2f);
            GetComponent<ParticleSystem>().Stop();

            yield return new WaitForSeconds(live_time / 2f);

            Callback();
            Destroy(gameObject);
        }
    }

    protected virtual void Callback() { }

}
