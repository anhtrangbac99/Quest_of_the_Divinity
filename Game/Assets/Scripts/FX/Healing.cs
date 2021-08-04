using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healing : MonoBehaviour
{
    private int type = 0;
    private int amount = 0;
    private GameObject target = null;
    private GameObject owner = null;

    public void SetOwner(GameObject owner)
    {
        this.owner = owner;
    }
    public void SetData(int target_type, GameObject target, int amount)
    {
        this.type = target_type;
        this.target = target;

        this.amount = amount;
    }

    public void Heal()
    {
        switch(type)
        {
            case 0:
                target.GetComponent<Minion>().ModifyLife(amount, true);
                break;
            case 1:
                target.GetComponent<Player>().Heal(amount);
                break;
        }
    }

    private void Update()
    {
        transform.position = owner.transform.position;
    }
}
