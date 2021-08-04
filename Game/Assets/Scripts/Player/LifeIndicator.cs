using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LifeIndicator : MonoBehaviour
{
    [SerializeField] private TMP_Text life_text = null;
    [SerializeField] private Image life_fill;

    void Start()
    {
        life_text.fontSharedMaterial.SetTextureOffset("_FaceTex", Vector2.down * 0.7f);
    }
    
    public void Indicate(int life, int maximum_life)
    {
        life_text.text = life.ToString();

        float old_fill = life_fill.fillAmount;
        life_fill.fillAmount = (float)life / maximum_life;

        if (old_fill <= 0.75f && old_fill >= 0.35f)
        {
            float offset = (0.7f - life_fill.fillAmount) / (0.7f - 0.355f);

            life_text.fontSharedMaterial.SetTextureOffset("_FaceTex", Vector2.down * 0.7f + Vector2.up * offset);
        }
    }
}
