using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GoldIndicator : MonoBehaviour
{
    private int last_active_gold_index = -1;
    [SerializeField] private int last_coin_x_pos = 800;
    [SerializeField] private Vector2 direction = Vector2.right;
    [SerializeField] private int base_gold_dist = 2;
    [SerializeField] private GameObject gold_coin;
    private List<GameObject> coins = null;
    [SerializeField] private TMP_Text gold_text = null;

    void Awake()
    {
        coins = new List<GameObject>();
    }

    public void Indicate(int current_gold, int maximum_gold)
    {
        gold_text.text = current_gold + "/" + maximum_gold;
        int current_gold_count = coins.Count;
        if (current_gold_count< current_gold)
        {
            for (int i = 0; i < current_gold - current_gold_count; i++)
            {
                GameObject on_set_coin = Instantiate(gold_coin, transform);
            
                on_set_coin.GetComponent<Transform>().position = GetComponent<Transform>().position; 

                coins.Add(on_set_coin);
            }            
        }

        SortCoins(current_gold);
    }

    private void SortCoins(int num_gold)
    {
        if (num_gold < last_active_gold_index)
        {
            for (int i = num_gold ; i < last_active_gold_index; i++)
            {
                coins[i].SetActive(false);
            }
        }
        else
        {
            // Double the base distance for left and right margin
            float gold_dist = gold_coin.GetComponent<RectTransform>().rect.width + base_gold_dist * 2;
            if (num_gold > 10)
            {
                Vector2 this_anchored_position = GetComponent<RectTransform>().anchoredPosition;

                gold_dist = Mathf.Abs((last_coin_x_pos - this_anchored_position.x) / num_gold);
            }
            
            for (int i = 0; i < coins.Count; i++)
            {
                if (i < num_gold)
                {
                    coins[i].SetActive(true);
                    coins[i].GetComponent<RectTransform>().anchoredPosition = direction * i * gold_dist;
                }
                else
                {
                    coins[i].SetActive(false);
                }
                
            }
        }

        last_active_gold_index = num_gold;
    }
}
