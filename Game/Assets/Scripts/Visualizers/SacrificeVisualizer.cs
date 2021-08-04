using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SacrificeVisualizer : Visualizer
{
    protected new Sacrifice card;
    protected override void Start()
    {
        base.Start();
        card = GetComponent<Sacrifice>();
    }
    public override void OnMouseFocus()
    {
        if (!card.IsActivated())
        {
            base.OnMouseFocus();
        }
    }
}
