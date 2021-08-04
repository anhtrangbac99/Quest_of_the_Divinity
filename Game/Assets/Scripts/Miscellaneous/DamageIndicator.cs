using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Image))]
public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text damge_indicator = null;
    [SerializeField] private Image damage_image;

    void Awake()
    {
        if (damage_image == null)
        {
            damage_image = GetComponent<Image>();
        }
    }
    public void Indicate(int damage, int damage_type)
    {
        if (damage <= 0) return;

        damge_indicator.text = "-" + damage;
        Sprite next_sprite = GameManager.GetInstance().GetStarBurstByDamageType(damage_type);
        damage_image.sprite = next_sprite;
        
        damage_image.color = Color.white;

        gameObject.SetActive(true);

        StartCoroutine(WaitAndDisappear());

        //----------------------------------------------
        IEnumerator WaitAndDisappear()
        {            
            yield return new WaitForSeconds(0.75f);
            float elapsed_time = 0f;

            Color color = damage_image.color;

            while (elapsed_time < 0.25f)
            {
                damage_image.color = new Color(color.r, color.g, color.b, 1f - (elapsed_time / 0.33f));

                elapsed_time += Time.deltaTime;
                yield return null;
            }

            damage_image.color = new Color(color.r, color.g, color.b, 0f);
            gameObject.SetActive(false);
        }
    }

    public void Break()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
}
