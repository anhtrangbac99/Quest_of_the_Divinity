using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShieldsIndicator : MonoBehaviour
{
    [SerializeField] private Image physical_shield;
    [SerializeField] private Image spell_shield;
    [SerializeField] private TMP_Text physical_text;
    [SerializeField] private TMP_Text spell_text;

    public void Indicate(int physical_value, int spell_value)
    {
        physical_text.text = physical_value.ToString();
        spell_text.text = spell_value.ToString();

        if (physical_value > 0 && !physical_shield.gameObject.activeInHierarchy)
        {
            physical_shield.gameObject.SetActive(true);
            StartCoroutine(Fade(physical_shield));
        }
        else if (physical_value <= 0 && physical_shield.gameObject.activeInHierarchy)
        {
            StartCoroutine(Fade(physical_shield, true));
        }

        if (spell_value > 0 && !spell_shield.gameObject.activeInHierarchy)
        {
            spell_shield.gameObject.SetActive(true);
            StartCoroutine(Fade(spell_shield));
        }
        else if (spell_value <= 0 && spell_shield.gameObject.activeInHierarchy)
        {
            StartCoroutine(Fade(spell_shield, true));
        }

        IEnumerator Fade(Image shield, bool fade_out = false)
        {
            Color color = shield.color;

            float elapsed_time = 0f;
            float fade_time = 1f;

            float start_point = (fade_out ? 1f : 0f);
            float end_point = (fade_out ? 0f : 1f);

            while(elapsed_time <= fade_time)
            {
                shield.color = new Color(color.r, color.g, color.b, Mathf.Lerp(start_point, end_point, (elapsed_time / fade_time)));
                elapsed_time += Time.deltaTime;

                yield return null;
            }

            shield.color = new Color(color.r, color.g, color.b, end_point);
            if (fade_out) shield.gameObject.SetActive(false);
        }
    }
}
