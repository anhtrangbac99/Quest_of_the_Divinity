using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oracle : Minion, IChampion, IPioneer
{
    [SerializeField] private GameObject particle;
    private GameObject real_life_particle = null;
    [SerializeField] private Effect reduce_activation_cost_effect = null;
    [SerializeField] private int modify_gold_amount = 1;

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();
        ActivateChampionEffect();
    }

    public override void Death()
    {
        ActivatePioneerEffect();
        base.Death();
    }
    public void ActivateChampionEffect()
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            owner.RetrieveCard();
        }
        else
        {
            StartCoroutine(WaitAndRetrieve());
        }

        IEnumerator WaitAndRetrieve()
        {
            float elapsed_time = 0f;
            float time = 1.5f;

            real_life_particle = Instantiate(particle, transform.position, Quaternion.identity);
            SpriteRenderer renderer = real_life_particle.GetComponent<SpriteRenderer>();
            Color color = renderer.color;

            while (elapsed_time < time)
            {
                renderer.color = new Color(color.r, color.g, color.b, Mathf.Lerp(1f, 0.05f, elapsed_time / time));
                elapsed_time += Time.deltaTime;

                yield return null;
            }

            DeactivateChampionEffect();
            owner.RetrieveCard();
        }
    }

    public void DeactivateChampionEffect()
    {
        if (real_life_particle != null)
        {
            Destroy(real_life_particle);
        }
    }

    public void ActivatePioneerEffect()
    {
        owner.AddEffectForPlayer(reduce_activation_cost_effect);
    }
}
