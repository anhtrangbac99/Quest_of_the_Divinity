using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Dragon : Minion
{
    protected override void Awake()
    {
        base.Awake();
        attack_time = 0.8f;
    }

    protected override void SetVisualizer()
    {
        visualizer = GetComponent<DragonVisualizer>();
    }

}
