using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{

    [SerializeField] private float time = 1.5f;
    private float start_time = 0f;
    
    void Start()
    {
        start_time = Time.time;
    }
    void Update()
    {
        if (Time.time - start_time >= time)
        {
            Destroy(gameObject);
        }
    }
}
