using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheOneAboveAll : Dragon, IChampion, IWarchief
{
    [SerializeField] private int initial_damage_to_enemies = 2;

    [SerializeField] private GameObject ripple_effect;
    //[SerializeField] private int num_life_on_replacing = 2;

    //[SerializeField] private bool attacking = false;
    [SerializeField] private Effect gain_damage_and_life_effect = null;
    [SerializeField] private AboveAllProjectile projectile = null;

    public override void OnActivateCallback()
    {
        base.OnActivateCallback();

        if (is_first_turn)
        {
            ActivateChampionEffect();
            ActivateWarchiefEffect();
        }        
    }

    public void ActivateChampionEffect()
    {

        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            DealDamageToEnemies();
        }
        else
        {
            StartCoroutine(WaitAndShake());
            Instantiate(ripple_effect, transform.position, transform.rotation);
        }        

        IEnumerator WaitAndShake()
        {
            yield return new WaitForSeconds(0.15f);
            Camera.main.GetComponent<CameraShake>().Shake(0.5f, 0.01f);

            DealDamageToEnemies();
        }

        void DealDamageToEnemies()
        {
            Player opponent = GameManager.GetInstance().GetOpponent(owner);
            List<Card> enemies = opponent.GetCardsOnBoard();

            if (enemies.Count > 0)
            {
                foreach (IDamagable e in enemies)
                {
                    e.OnReceiveDamage(null, initial_damage_to_enemies, 0);
                }
            }

            opponent.OnReceiveDamage(null, initial_damage_to_enemies, 0);
        }
    }

    public override void Strike(bool duel = false)
    {
        MonoBehaviour _ = target as MonoBehaviour;
        Vector3 target_position = _.transform.position;

        int final_phys_damage, final_spell_damage;
        CalculateFinalDamage(out final_phys_damage, out final_spell_damage);

        AboveAllProjectile proj = Instantiate(projectile, transform.position, Quaternion.identity);
        proj.Throw(target, target_position, final_phys_damage, final_spell_damage, 2.5f);

        if (owner.attack_notification != null)
        {
            owner.attack_notification(this, target);
        }

        OnStrikeCallback(target, final_phys_damage, final_spell_damage);
    }

    protected override void OnStrikeCallback(IDamagable target, int physical_damage, int spell_damage)
    {
        this.target = null;
    }

    public override void Death()
    {
        DeactivateWarchiefEffect();
        base.Death();
    }

    public void DeactivateChampionEffect()
    {
        
    }

    public void ActivateWarchiefEffect()
    {
        owner.AddEffectForMinion(gain_damage_and_life_effect);
    }

    public void DeactivateWarchiefEffect()
    {
        owner.RemoveEffectFromMinion(gain_damage_and_life_effect);
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/
    protected override bool HasAvailableSpecialEffect()
    {
        // As it grant other dragons damage & life
        return owner.GetBoardSide().HasDragon(true);
    }
}

