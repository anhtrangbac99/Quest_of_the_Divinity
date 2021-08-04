using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Freeze : Effect
{
    [SerializeField] private bool no_shake = false;

    public override void TakeEffect()
    {
        MinionVisualizer visualizer = attached_target.GetComponent<MinionVisualizer>();
        Minion minion = attached_target.GetComponent<Minion>();
        if (minion != null)
        {
            visualizer.SwitchShader("Shader Graphs/Freezing");
            if (!(no_shake && GameManager.GetInstance().NON_GRAPHICAL))
            {
                StartCoroutine(visualizer.Shake(1f, 0.25f, 0.1f));
            }

            minion.ModifyWaitTurns(1);
        }
    }

    public override void OnNewTurnUpdate()
    {
        base.OnNewTurnUpdate();
        no_shake = false;

        if (activated)
        {
            TakeEffect();
        }
    }

    public override void RevertEffect()
    {
        base.RevertEffect();
        MinionVisualizer visualizer = attached_target.GetComponent<MinionVisualizer>();
        visualizer?.SwitchShader("Shader Graphs/Outline");
    }
}
