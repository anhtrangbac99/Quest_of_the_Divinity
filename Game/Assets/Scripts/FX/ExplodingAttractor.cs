using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingAttractor : Attractor
{
    [SerializeField] private GameObject explosion;
    private Vector3 position;
    private IDamagable target;
    private int physical_damage = 0;
    private int spell_damage = 0;

    public void SetTarget(IDamagable target, Vector3 position, int physical_damage, int spell_damage)
    {
        this.physical_damage = physical_damage >= 0 ? physical_damage : 0;
        this.spell_damage = spell_damage >= 0 ? spell_damage : 0;
        this.target = target;
        this.position = position;

        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            target.OnReceiveDamage(null, this.physical_damage, this.spell_damage);
            Destroy(gameObject);
        }
        else
        {
            Move(position, 2f);
        }
    }

    protected override void Callback()
    {
        Instantiate(explosion, position, Quaternion.identity);
        target.OnReceiveDamage(null, physical_damage, spell_damage);
    }
}
