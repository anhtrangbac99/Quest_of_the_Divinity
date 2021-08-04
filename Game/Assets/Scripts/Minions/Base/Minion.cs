using System.Collections.Generic;
using UnityEngine;

public abstract class Minion : Card, IDamagable, ITurnUpdatable
{
    [SerializeField] protected MinionClass minion_class = MinionClass.None;
    protected Dictionary<string, Effect> effects = null;

    [SerializeField] protected GameObject custom_attack_effect = null;

    protected new MinionVisualizer visualizer;

    protected float attack_time;

    /* Critical strike stats */
    [SerializeField] protected float critical_strike_modifier = 0.5f;
    [SerializeField] protected int maximum_critical_chance = 80;
    protected int critical_chance = 0;

    /* Base stats */
    protected int wait_turns = 1;
    [SerializeField] protected int life = 1;
    [SerializeField] protected int base_maximum_life = 1;
    [SerializeField] protected int physical_damage = 1;
    [SerializeField] protected int spell_damage = 0;
    [SerializeField] protected int added_physical_damage = 0;
    [SerializeField] protected int added_spell_damage = 0;
    [SerializeField] protected int added_life = 0;

    [SerializeField] protected bool is_guard = false;
    [SerializeField] protected bool is_raider = false;
    [SerializeField] protected bool is_first_turn = true;
    protected bool log__log_on_activate = true;


    //-------------------------------------------------
    protected virtual void Awake()
    {
        attack_time = 0.75f;
        is_first_turn = true;
    }

    protected override void Start()
    {
        base.Start();

        visualizer.SetName(card_name_in_string);
        visualizer.SetCost(cost);

        ShowDamageAndLife();
    }

    public override void Activate(bool using_gold = true)
    {
        if (owner.PlaceThisCard(this, true, using_gold))
        {        
            // Set this minion's current life to its base maximum life
            life = base_maximum_life;

            // Set base stats
            critical_chance = 0;
            added_physical_damage = 0;
            added_spell_damage = 0;
            added_life = 0;

            // Initialize the effect list
            effects = new Dictionary<string, Effect>();

            // Raider can attack immediately
            if (is_raider) wait_turns = 0;
            else wait_turns = 1;      

            if (is_guard)
            {
                owner.AddNewGuard(this);
            }

            if (visualizer == null)
            {
                SetVisualizer();
            }

            visualizer.OnActivateSetUp();
            // Only show animation with minions activated with golds
            visualizer.ChangeCardAppearance(true, using_gold);

            ShowDamageAndLife();

            // Fire the minion related effects in this player
            owner.ApplyAllEffect(this);
            // Raise the activation flag
            activated = true;

            if (log__log_on_activate && GameManager.GetInstance().LOG)
            {
                GameManager.GetInstance().LogPlayCard(log__card_position);
            }
        }
        else
        {
            activated = false;
            visualizer.OnEndDrag(null);
        }
    }

    public override void Deactivate(bool returning_gold = true)
    {
        owner.RemoveThisCardFromBoard(this);
        owner.AddCardToHand(this);
        if (returning_gold)
        {
            owner.AddGold(cost);
        }

        visualizer.ChangeCardAppearance(false);

        activated = false;
        foreach (Effect e in effects.Values)
        {
            e.RevertEffect();
        }
    }

    protected override void SetVisualizer()
    {
        visualizer = GetComponent<MinionVisualizer>();
    }

    //-------------------------------------------------
    protected void CalculateFinalDamage(out int physical, out int spell, bool api = false)
    {
        // Calculate final damage!
        int final_phys_damage = physical_damage + added_physical_damage;
        int final_spell_damage = spell_damage + added_spell_damage;

        if (!api && PercentageRNG.Chance(critical_chance - 1))
        {
            Debug.Log("Crit");
            float final_modifier = 1f + critical_strike_modifier;
            // If this minion has both physical and spell damage
            if (final_phys_damage + final_spell_damage > final_phys_damage)
            {
                // Split the crit modifier to both of them
                final_modifier -= critical_strike_modifier / 2f;
            }
            
            // Recalculate the final damages
            final_phys_damage = (int)(final_modifier * final_phys_damage);
            final_spell_damage = (int)(final_modifier * final_spell_damage);
        }

        physical = final_phys_damage;
        spell = final_spell_damage;
    }

    public void DealDamage(IDamagable target)
    {
        MonoBehaviour _target = target as MonoBehaviour;
        Transform target_transform = _target.transform;
        
        wait_turns = 1;
        visualizer.EvaluateOutline(OutlineChangeType.Ready);

        GameManager.GetInstance().NotifyAttackIsOver();

        visualizer.AttackMove(target_transform.position, attack_time);
    }

    public virtual void Strike(bool duel = false)
    {
        int physical_damage, spell_damage;
        CalculateFinalDamage(out physical_damage, out spell_damage);

        if (owner.attack_notification != null)
        {
            owner.attack_notification(this, target);
        }

        // Call the OnReceiveDamage from the target
        target.OnReceiveDamage((duel ? null : this), physical_damage, spell_damage);

        OnStrikeCallback(target, physical_damage, spell_damage);
    }

    public void FinalDuel(TheFinalDuel duel_card, Minion target)
    {
        this.target = target as IDamagable;

        visualizer.AttackMove(target.transform.position, attack_time, duel_card, target);        
    }

    public void FinalDuelCallback(TheFinalDuel duel_card, Minion target)
    {
        if (target.IsDead())
        {
            duel_card.ActivateEffectOnwinnerSide(this);

            Death();
        }
        else
        {
            target.FinalDuel(duel_card, this);
        }
    }
    
    public virtual void OnReceiveDamage(IDamagable source, int physical_damage, int spell_damage)
    {
        int calc_damage = physical_damage + spell_damage;

        added_life -= calc_damage;
        life = (added_life < 0 ? life + added_life : life);

        if (added_life < 0)
        {
            owner.minion_receive_damage_notifier?.Invoke(this, added_life);            
            added_life = 0;
        }

        int damage_type = (physical_damage > 0 ? 1 : 0) + (spell_damage > 0 ? 2 : 0);

        visualizer.IndicateReceiveDamage(calc_damage, damage_type, (life + added_life));

        if (source != null)
        {
            int final_phys_damage = this.physical_damage + added_physical_damage;
            int final_spell_damage = this.spell_damage + added_spell_damage;
            source.OnReceiveDamage(null, final_phys_damage, final_spell_damage);
        }

        if (this.life <= 0)
        {
            this.life = 0;

            visualizer.IndicateBaseLife(0);

            if (GameManager.GetInstance().NON_GRAPHICAL)
            {
                InvokeDeath();
            }
            else
            {
                Invoke("StopAllCoroutines", 0.25f);
                Invoke("InvokeDeath", 0.3f);
            }
        }
    }

    public void ScatterDebris(int damage, GameObject custom_debris = null)
    {
        visualizer.ScatterDebris(damage, custom_debris);
    }
    //-------------------------------------------------

    public virtual void Death() 
    {
        if (gameObject.activeInHierarchy)
        {
            visualizer.DeathAnimation(0.35f, 0.2f * (1f - ((float)life / base_maximum_life)), 1f / 55f, false);
        }

        // Destroy(gameObject);
    }

    public void RemoveMinionOnDeath()
    {
        if (GameManager.GetInstance().minion_dies_notifier != null)
        {
            GameManager.GetInstance().minion_dies_notifier(owner);
        }

        owner.RemoveThisCardFromBoard(this);
        owner.UpdateCardStatus(APIDirector.GetInstance().GetCardID(GetCardName()), UsedCardStatus.Used);

        GameManager.GetInstance().switch_on();
        gameObject.SetActive(false);
    }

    public void ForceDeath(bool sort = false)
    {
        owner.RemoveThisCardFromBoard(this, sort);
        gameObject.SetActive(false);
    }

    public bool IsDead() => life <= 0;
    
    //-------------------------------------------------

    public MinionClass GetClass()
    {
        return minion_class;
    }

    public bool IsActivated() => activated;

    public bool IsGuard()
    {
        return is_guard;
    }

    public bool IsWounded() => (life + added_life < base_maximum_life);

    public bool IsFreezing()
    {
        foreach (Effect e in effects.Values)
        {
            if (e is Freeze && e.IsActivating())
            {
                return true;
            }
        }

        return false;
    }

    public bool IsFirstTurn() => is_first_turn;
    public int GetWaitTurns() => wait_turns;

    public int GetCurrentLife() => (life + added_life);

   //-------------------------------------------------

    public virtual void ModifyDamage(int amount, bool is_permanent = false)
    {
        added_physical_damage += amount;

        /****/
        int final_damage = physical_damage + added_physical_damage;
        visualizer.IndicateBaseDamage(final_damage);
    }

    public virtual void ModifyLife(int amount, bool life_only = false)
    {
        if (amount > 0)
        {
            life += amount;
            if (life > base_maximum_life)
            {
                if (!life_only)
                {
                    this.added_life += life - base_maximum_life;
                }
                life = base_maximum_life;            
            }        
        }
        else 
        {
            if (added_life > 0)
            {
                added_life += amount;
                added_life = 0;
            }
        }

        visualizer.IndicateBaseLife(life + added_life);
    }

    public void SetLife(int to)
    {
        if (to <= 0) return;
        life = to;
        added_life = 0;

        visualizer.IndicateBaseLife(life + added_life);
    }

    public void ModifyWaitTurns(int amount)
    {
        wait_turns += amount;
    }

    public void ModifyCriticalChance(int amount)
    {
        critical_chance += amount;
        if (critical_chance >= maximum_critical_chance)
        {
            critical_chance = maximum_critical_chance;
        }
        else if (critical_chance < 0)
        {
            critical_chance = 0;
        }
    }
    public virtual void AddEffect(Effect effect)
    {
        string effect_id = effect.GetID();

        // If this effect is not affecting this target
        if (!effects.ContainsKey(effect_id))
        {
            // Create an object from this effect 
            Effect object_effect = Instantiate(effect) as Effect;
            
            // Add it
            object_effect.AssignTarget(gameObject);
            effects.Add(effect_id, object_effect);
        }
        else
        {
            // Else, reset its time to max
            effects[effect_id].ResetRemainingTurn();
        }
    }

    public void AddGuardAttribute()
    {
        is_guard = true;

        visualizer.VisualizeGuard();

        owner.AddNewGuard(this);
    }

    public void RemoveGuardAttribute()
    {
        is_guard = false;

        visualizer.VisualizeGuard();

        owner.RemoveGuard(this);
    }
    
    //-------------------------------------------------

    protected virtual void ShowDamageAndLife()
    {
        visualizer.IndicateBaseDamage(physical_damage);
        visualizer.IndicateBaseLife(life);
    }

    //-------------------------------------------------
    public void NotifyCardIsClicked()
    {
        GameManager.GetInstance().NotifyCardIsClicked(this, owner);
    }

    /************************************
    *                                   *
    *       IUPDATABLE SECTION          *
    *                                   *
    ************************************/
    public void RegisterTurnUpdatable()
    {
        GameManager.GetInstance().RegisterTurnUpdatable(this);
    }

    public void UnregisterTurnUpdatable()
    {
        GameManager.GetInstance().UnregisterTurnUpdatable(this);
    }

    public virtual void OnNewTurnUpdate()
    {
        is_first_turn = false;
        wait_turns = 0;
        foreach(KeyValuePair<string, Effect> kvp in effects)
        {
            kvp.Value.GetComponent<ITurnUpdatable>().OnNewTurnUpdate();
        }

        visualizer.EvaluateOutline(OutlineChangeType.Ready);
    }


    /************************************
    *                                   *
    *          CALLBACK SECTION         *
    *                                   *
    ************************************/

    public virtual void OnActivateCallback()
    {
        visualizer.VisualizeDropDownEffect();
        visualizer.ToggleFront(true);
    }
    public virtual void OnDeathCallback() 
    {
        visualizer.VisualizeDeathEffect();
    }

    protected virtual void OnStrikeCallback(IDamagable target, int physical_damage, int spell_damage)
    {
        target.ScatterDebris(physical_damage + spell_damage, custom_attack_effect);

        this.target = null;
    }


    /************************************
    *                                   *
    *       ASSIGN TARGET SECTION       *
    *                                   *
    ************************************/

    public override void AssignTarget(IDamagable target)
    {
        base.AssignTarget(target);
        if (wait_turns <= 0)
        {
            if (GameManager.GetInstance().LOG)
            {
                log__card_position = owner.GetMinionsOnBoard().IndexOf(this) + Constants.LOG__CHOOSING_CARD_OFFSET;

                GameManager.GetInstance().LogCardAttack(log__card_position, log__target_position);
            }
            DealDamage(target);
        }
    }

    protected void InvokeDeath()
    {
        Death();
    }

    /************************************
    *                                   *
    *            API SECTION            *
    *                                   *
    ************************************/

    public override CardData RetrieveCardData()
    {
        int i = APIDirector.GetInstance().GetCardID(card_name_in_string);

        if (!activated)
        {
            return new CardData(true, i);
        }
        else
        {
            int physical, spell;
            CalculateFinalDamage(out physical, out spell, true);

            int status = wait_turns == 0 ? 2 : 0;
            if (status == 2 && HasAvailableSpecialEffect())
            {
                status = 1;
            }

            return new CardData(true, i, status, physical + spell, life + added_life);
        }
    }

    public int GetDamage() => physical_damage + added_physical_damage + spell_damage + added_spell_damage;

    protected virtual bool HasAvailableSpecialEffect() => false;
}
