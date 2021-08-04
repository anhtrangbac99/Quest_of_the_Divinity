using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MinionVisualizer : Visualizer, IMouseDownHandler
{
    [SerializeField] protected TMP_Text card_damage = null;
    [SerializeField] protected TMP_Text card_life = null;

    [SerializeField] protected DamageIndicator damage_indicator = null;

    [SerializeField] protected AnimationCurve clip;

    [SerializeField] protected GameObject info = null;
    [SerializeField] protected GameObject onboard_minion = null;
    [SerializeField] protected GameObject onboard_front = null;
    [SerializeField] protected GameObject activation_effect = null;
    [SerializeField] protected GameObject dropped_effect = null;
    [SerializeField] protected GameObject damaged_effect = null;
    [SerializeField] protected GameObject attack_effect = null;
    [SerializeField] protected GameObject death_effect = null;

    protected new Minion card;

    protected IEnumerator attack_coroutine;

    protected Vector3 move_to;

    [SerializeField] protected float scale_on_activation = 0.6f;

    protected override void Awake()
    {
        base.Awake();
        transform.Find("Canvas").gameObject.SetActive(true);

        SwitchOn();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        // Getting the minion in this card
        card = GetComponent<Minion>();

        Player owner = card.GetOwner();

        if (activation_effect == null)
        {
            activation_effect = owner.GetBasicActivationEffect();
        }

        if (dropped_effect == null)
        {
            dropped_effect = owner.GetBasicDropEffect();
        }

        if (death_effect == null)
        {
            death_effect = owner.GetBasicDeathEffect();
        }

        if (damaged_effect == null)
        {
            damaged_effect = owner.GetBasicDamagedEffect();
        }
    }

    /// <summary>
    /// Evaluate the minion's outline based on the outline type
    /// <para>
    /// Parameters:
    /// <para> <paramref name="type"/>: Outline Change Type enum. </para>
    /// <para> <paramref name="wait_turns"/>: current wait turns of the minion. </para>
    /// <para> <paramref name="is_guard"/>: is this minion a guard? </para>
    /// </para>
    /// </summary>
    public void EvaluateOutline(OutlineChangeType type)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
            return;

        int wait_turns = card.GetWaitTurns();
        bool is_guard = card.IsGuard();

        Material mat = onboard_front.GetComponentInChildren<SpriteRenderer>().material;

        if (!mat.shader.name.Equals("Shader Graphs/Outline")) return;

        switch (type)
        {
            case OutlineChangeType.Attack:
                mat.SetColor("_OutlineColor", Color.red);
                mat.SetFloat("_OutlineThickness", 3f);
                mat.SetVector("_Blinking", new Vector2(0, 1));
                break;
            case OutlineChangeType.Ready:
                mat.SetColor("_OutlineColor", Color.green);
                mat.SetFloat("_OutlineThickness", (wait_turns == 0 ? 3f : 0f));
                mat.SetVector("_Blinking", new Vector2(1, 1));
                break;
            case OutlineChangeType.Defense:
                if (is_guard)
                {
                    mat.SetColor("_OutlineColor", Color.yellow);
                    mat.SetFloat("_OutlineThickness", 3f);
                    mat.SetVector("_Blinking", new Vector2(1, 1));
                }
                break;
            case OutlineChangeType.Off:
                mat.SetFloat("_OutlineThickness", 0f);
                break;
        }
        mat.SetInt("_IsGuard", (is_guard ? 1 : 0));
    }

    protected virtual void OnActivateFallDown()
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);

            card.OnActivateCallback();
        }
        else
        {
            Vector3 position = transform.position;
            transform.position = new Vector3(position.x, position.y, -1.25f);

            StartCoroutine(ChangeRotation(.25f));
        }

        IEnumerator ChangeRotation(float time)
        {
            Quaternion current_rotation = transform.rotation;
            Quaternion last_rotation = Quaternion.Euler(15f, -10f, 0f);
            GetComponent<Rigidbody>().isKinematic = false;

            float elapsed_time = 0f;
            while (elapsed_time < time)
            {
                transform.rotation = Quaternion.Lerp(current_rotation, last_rotation, elapsed_time / time);

                elapsed_time += Time.deltaTime;
                yield return null;
            }

            yield return new WaitForSeconds(0.25f);

            transform.rotation = Quaternion.Euler(0, 0, 0);
            GetComponent<Rigidbody>().isKinematic = true;

            if (GameManager.GetInstance().GetCurrentPlayerTurn() != card.GetOwner())
            {
                EvaluateOutline(OutlineChangeType.Defense);
            }
            else
            {
                EvaluateOutline(OutlineChangeType.Ready);
            }

            card.OnActivateCallback();
        }
    }

    public void Move(Vector3 to, float time, bool onboard)
    {
        if (card == null)
        {
            card = GetComponent<Minion>();
        }

        if (!card.IsActivated() && onboard)
        {
            move_to = to;
        }
        else
        {
            base.Move(to, time);
        }
    }

    public void AttackMove(Vector3 target_position, float attack_time)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            card.Strike();
        }
        else
        {
            attack_coroutine = AttackTarget(target_position, attack_time);
            StartCoroutine(attack_coroutine);
        }
    }

    public void AttackMove(Vector3 target_position, float attack_time, TheFinalDuel duel_card, Minion target)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            card.Strike(true);
            card.FinalDuelCallback(duel_card, target);            
        }
        else
        {
            attack_coroutine = AttackTarget(target_position, attack_time, duel_card, target);
            StartCoroutine(attack_coroutine);
        }
    }

    public void StopAttack()
    {
        if (attack_coroutine != null)
        {
            StopCoroutine(attack_coroutine);
        }
    }
    
    public void DeathAnimation(float time, float magnitude, float time_interval, bool force_death = false)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            card.OnDeathCallback();
            card.RemoveMinionOnDeath();

            GameManager GM = GameManager.GetInstance();

            GM.switch_on -= SwitchOn;
            GM.switch_off -= SwitchOff;
        }
        else
        {
            StartCoroutine(ShakeAndDeath(time, magnitude, time_interval, force_death));
        }
    }
    
    public override void OnActivateSetUp()
    {

        base.OnActivateSetUp();

        GameManager GM = GameManager.GetInstance();

        GM.switch_on += SwitchOn;
        GM.switch_off += SwitchOff;
    }

    public DamageIndicator GetDamageIndicator() => damage_indicator;

    /********************************
     *                              *
     *        STATS SECTION         *
     *                              *
     ********************************/
    public void IndicateBaseDamage(int damage)
    { 
        card_damage.text = damage.ToString();
    }

    public void IndicateBaseLife(int life)
    {
        card_life.text = life.ToString();
    }

    public void IndicateReceiveDamage(int calc_damage, int damage_type, int new_life)
    {
        damage_indicator.Indicate(calc_damage, damage_type);
        card_life.text = new_life.ToString();
    }

    public void ScatterDebris(int damage, GameObject custom_debris = null)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
            return;

        if (custom_debris != null)
        {
            Instantiate(custom_debris, transform.position, Quaternion.identity);
            // TO DO WITH THE CUSTOM DEBRIS
        }
        else
        {
            ParticleSystem p = Instantiate(damaged_effect, transform.position, Quaternion.identity).GetComponent<ParticleSystem>();

            // Set a burst for the particle
            var em = p.emission;
            int additional_particle_burst = (damage > 7 ? (int)(damage * 1.5f) : 0);
            em.SetBursts(
                new ParticleSystem.Burst[]
                {
                    new ParticleSystem.Burst(0.0f, 10 + additional_particle_burst)
                }
            );

            // Set the direction
            var s = p.velocityOverLifetime;

            float y_velocity = (damage > 7 ? 2 : 1) * (transform.position.y > 0f ? 1 : -1);
            s.y = y_velocity;

        }
    }

    /********************************
     *                              *
     *       GRAPHIC SECTION        *
     *                              *
     ********************************/

    public void VisualizeGuard()
    {
        bool is_guard = card.IsGuard();

        SpriteRenderer front = onboard_front.GetComponent<SpriteRenderer>();
        front.sprite = GameManager.GetInstance().GetGuardFront();
        front.material.SetInt("_IsGuard", (is_guard ? 1 : 0));

        float new_scale = (is_guard ? 1.25f : 1f);
        onboard_front.transform.localScale = new Vector3(new_scale, new_scale, 1f);
    }

    public void ChangeCardAppearance(bool onboard = true, bool animated = true)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            SetAppearance();
        }
        else
        {
            if (onboard && animated)
            {
                StartCoroutine(OnBoardChangeAlpha());
                return;
            }

            SetAppearance();
        }

        IEnumerator OnBoardChangeAlpha()
        {
            float elapsed_time = 0f;
            float time = 0.5f;
            bool instantiated = false;

            transform.localScale = new Vector3(focus_scale, focus_scale, 0.01f);

            while (elapsed_time < time)
            {
                ChangeInfoAlpha(null, Mathf.Lerp(1f, 0f, elapsed_time / time));

                if (!instantiated && elapsed_time >= time / 2f)
                {
                    Instantiate(activation_effect, transform.position, transform.rotation);
                    instantiated = true;
                }

                elapsed_time += Time.deltaTime;

                yield return null;
            }

            SetAppearance();
        }

        void SetAppearance()
        {
            info.SetActive(!onboard);
            onboard_minion.SetActive(onboard);


            foreach (TMP_Text t in info_texts)
            {
                if (!t.name.Equals("Damage") && !t.name.Equals("Life") && !t.name.Equals("Text"))
                {
                    t.gameObject.SetActive(!onboard);
                }
            }

            if (onboard)
            {
                transform.position = move_to;
                transform.localScale = new Vector3(scale_on_activation, scale_on_activation, transform.localScale.z);
                ChangeInfoAlpha(null, 1f);

                if (card.IsFirstTurn())
                    OnActivateFallDown();
            }
            else
            {
                transform.localScale = new Vector3(scale_on_hand, scale_on_hand, transform.localScale.z);
            }
        }
    }

    public void VisualizeDropDownEffect()
    {
        if (!GameManager.GetInstance().NON_GRAPHICAL)
        {
            GameObject _ = Instantiate(dropped_effect, transform.position, Quaternion.identity);
            _.transform.parent = transform;
        }
    }

    public void VisualizeDeathEffect()
    {
        if (!GameManager.GetInstance().NON_GRAPHICAL)
        {
            GameObject _ = Instantiate(death_effect, transform.position, Quaternion.identity);
            ParticleSystem p = _.GetComponent<ParticleSystem>();
            if (p != null && p.isStopped)
            {
                p.Play();
            }
        }

        onboard_minion.SetActive(false);
        transform.Find("Canvas").gameObject.SetActive(false);
    }
    /************************************
    *                                   *
    *            DRAG SECTION           *
    *                                   *
    ************************************/

    public override void OnBeginDrag(PointerEventData data)
    {
        if (!card.IsActivated())
        {
            base.OnBeginDrag(data);
        }
    }

    public override void OnDrag(PointerEventData data)
    {
        if (!card.IsActivated())
        {
            base.OnDrag(data);
        }
    }

    public override void OnEndDrag(PointerEventData data)
    {
        if (!card.IsActivated())
        {
            base.OnEndDrag(data);
        }
    }

    /************************************
    *                                   *
    *         IMOUSEDOWN SECTION        *
    *                                   *
    ************************************/

    public virtual void OnMouseClickDown()
    {
        if (card.IsActivated())
        {
            GameManager GM = GameManager.GetInstance();

            if (GM.GetCurrentGameState() == GameState.None && GM.GetCurrentPlayerTurn() == card.GetOwner() && card.GetWaitTurns() == 0)
            {
                EvaluateOutline(OutlineChangeType.Attack);
            }

            card.NotifyCardIsClicked();
        }
    }

    public virtual void OnAbort()
    {
        if (card.IsActivated())
        {
            EvaluateOutline(OutlineChangeType.Ready);
        }
    }

    /************************************
    *                                   *
    *           FOCUS SECTION           *
    *                                   *
    ************************************/

    public override void OnMouseFocus()
    {
        if (!card.IsActivated())
        {
            base.OnMouseFocus();
        }
    }
    /************************************
    *                                   *
    *       SHADER SWITCH SECTION       *
    *                                   *
    ************************************/
    public void SwitchShader(string shader_name)
    {
        Material mat = onboard_front.GetComponent<SpriteRenderer>().material;
        if (!mat.shader.name.Equals(shader_name))
        {
            mat.shader = Shader.Find(shader_name);
        }
    }

    /************************************
    *                                   *
    *         COROUTINE SECTION         *
    *                                   *
    ************************************/
    protected virtual IEnumerator AttackTarget(Vector3 target_position, float attack_time, TheFinalDuel duel_card = null, Minion target = null)
    {
        bool attacked = false;

        float elapsed_time = 0f;
        Vector3 last_position = transform.position;

        GameManager.GetInstance().switch_off();

        while (elapsed_time <= attack_time)
        {
            transform.position = Vector3.Lerp(last_position, target_position, clip.Evaluate(elapsed_time / attack_time));

            if (!attacked && elapsed_time >= attack_time / 3.5f)
            {
                card.Strike((duel_card != null));
                attacked = true;
            }

            elapsed_time += Time.deltaTime;

            yield return null;
        }

        GameManager.GetInstance().switch_on();

        transform.position = last_position;

        if (duel_card != null)
        {
            card.FinalDuelCallback(duel_card, target);
        }
    }

    protected virtual IEnumerator ShakeAndDeath(float time, float magnitude, float time_interval, bool force_death = false)
    {
        SwitchOff();

        float elapsed_time = 0f;

        Vector3 original_position = transform.position;

        while (elapsed_time < time)
        {
            float percent_complete = elapsed_time / time;
            float damper = 1f - Mathf.Clamp(4f * percent_complete - 3f, 0f, 1f);

            float pos_x = Random.Range(-1f, 1f);
            float pos_y = Random.Range(-1f, 1f);

            pos_x *= magnitude * damper;
            pos_y *= magnitude * damper;

            transform.position = original_position + new Vector3(pos_x, pos_y, 0f);

            elapsed_time += time_interval;

            yield return new WaitForSeconds(time_interval);
        }

        card.OnDeathCallback();

        yield return new WaitForSeconds(0.33f);

        transform.position = original_position;

        card.RemoveMinionOnDeath();

        GameManager GM = GameManager.GetInstance();

        GM.switch_on -= SwitchOn;
        GM.switch_off -= SwitchOff;
    }

    public IEnumerator Shake(float time, float magnitude, float time_interval)
    {
        float elapsed_time = 0f;

        Vector3 original_position = transform.position;

        while (elapsed_time < time)
        {
            float percent_complete = elapsed_time / time;
            float damper = 1f - Mathf.Clamp(4f * percent_complete - 3f, 0f, 1f);

            float pos_x = Random.Range(-1f, 1f);
            float pos_y = Random.Range(-1f, 1f);

            pos_x *= magnitude * damper;
            pos_y *= magnitude * damper;

            transform.position = original_position + new Vector3(pos_x, pos_y, 0);

            elapsed_time += time_interval;

            yield return new WaitForSeconds(time_interval);
        }

        transform.position = original_position;
    }
}
