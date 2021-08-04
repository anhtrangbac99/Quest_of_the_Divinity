using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpellObject : MonoBehaviour
{
    [SerializeField] protected bool require_target = true;
    [SerializeField] protected int spell_damage = 2;
    protected Player player = null;
    public abstract void AssignTarget(Card card);

    public abstract void AssignPlayer(Player player);

    public bool NeedsTarget() => require_target;
}
