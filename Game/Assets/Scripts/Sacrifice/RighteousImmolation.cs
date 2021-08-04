using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RighteousImmolation : Sacrifice
{
    private MinionClass destroyed_card_class = MinionClass.None;
    [SerializeField] private Effect sage_effect = null;
    [SerializeField] private Effect warrior_effect = null;
    [SerializeField] private Effect abyss_effect = null;
    [SerializeField] private Effect nature_effect = null;
    [SerializeField] private Effect dragon_effect = null;

    public override void AssignTarget(Card card)
    {
        destroyed_card_class = card.GetComponent<Minion>().GetClass();
        base.AssignTarget(card);
    }

    protected override void TakeEffect()
    {
        switch(destroyed_card_class)
        {
            case MinionClass.Sage:
                owner.AddEffectForPlayer(sage_effect);
                break;
            case MinionClass.Warrior:
                owner.AddEffectForMinion(warrior_effect);
                break;
            case MinionClass.Abyss:
                owner.AddEffectForPlayer(abyss_effect);
                break;
            case MinionClass.Nature:
                owner.AddEffectForPlayer(nature_effect);
                break;
            case MinionClass.Dragon:
                owner.AddEffectForPlayer(dragon_effect);
                break;
            default:
                effect_reference = null;
                break;
        }
        base.TakeEffect();
    }
}
