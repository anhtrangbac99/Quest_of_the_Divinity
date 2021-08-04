using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellVisualizer : Visualizer
{
    protected new Spell card;
    protected override void Start()
    {
        base.Start();
        card = GetComponent<Spell>();
    }

}
