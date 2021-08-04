using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;


public abstract class Visualizer : MonoBehaviour, ISwitch, IDragHandler, IBeginDragHandler, IEndDragHandler, IMouseFocusHandler
{
    public bool IsFocusing
    {
        get { return isFocusing; }
        private set { isFocusing = value; }
    }
    protected Vector3 begin_drag_position = Vector3.zero;
    protected Vector3 non_focus_scale;

    protected List<Image> info_images = null;
    protected List<TMP_Text> info_texts = null;

    protected Card card;

    protected bool isFocusing = false;
    protected bool drew = false;

    protected int focus_raise = 100;

    protected float focus_scale = 1.15f;
    protected float scale_on_hand = 0.5f;

    protected float drag_multiplier_x = 30f;
    protected float drag_multiplier_y = 20f;

    protected float squared_dragged_distance_to_activate_card = 1f;

    [SerializeField] protected GameObject canvas_front = null;
    [SerializeField] protected GameObject canvas_back = null;


    protected virtual void Awake()
    {
        GetComponent<Rigidbody>().isKinematic = true;

        drew = false;

        focus_scale = 1.5f;
        focus_raise = 100;
        isFocusing = false;

        scale_on_hand = 0.75f;

        drag_multiplier_x = 30f;
        drag_multiplier_y = 20f;

        squared_dragged_distance_to_activate_card = 10f;

        // Get all info images to this list
        info_images = GetComponentsInChildren<Image>().ToList();

        // Get all TMP_Text to this list
        info_texts = GetComponentsInChildren<TMP_Text>().ToList();
    }
    protected virtual void Start()
    {
        card = GetComponent<Card>();
    }

    public virtual void OnActivateSetUp()
    {
        if (isFocusing)
        {
            OnMouseFocus();
        }
    }

    //--------------------------------------------------
    public void SetName(string name)
    {
        StartCoroutine(WaitAndSetText("Name", name));
    }

    public void SetCost(int cost)
    {
        StartCoroutine(WaitAndSetText("Cost", cost.ToString()));
    }

    IEnumerator WaitAndSetText(string predicate, string text)
    {
        while (info_texts.Count <= 0)
        {
            info_texts = GetComponentsInChildren<TMP_Text>().ToList();

            yield return null;
        }

        info_texts.Find(x => x.name.Equals(predicate)).text = text;
        yield return 0;
    }

    //--------------------------------------------------
    public virtual void Move(Vector3 to, float time)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            transform.position = to;
        }
        else
        {
            StartCoroutine(MoveCard(to, time));
        }
    }

    public virtual void ToggleFront(bool show)
    {
        canvas_front.SetActive(show);
        canvas_back.SetActive(!show); 
    }

    public void MoveToHand(Vector3 to, Vector3 sort_point, float show_time, float turn_time, float move_time)
    {
        if (GameManager.GetInstance().NON_GRAPHICAL)
        {
            transform.position = to;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else
        {
            StartCoroutine(AddCardToHand(to, sort_point, show_time, turn_time, move_time));
        }
    }

    protected void ReoderCanvas(int order_change)
    {
        GetComponentInChildren<Canvas>().sortingOrder += order_change;
    }

    /************************************
    *                                   *
    *           ISWITCH SECTION         *
    *                                   *
    ************************************/

    public void SwitchOn()
    {
        GetComponent<Collider>().enabled = true;
    }

    public void SwitchOff()
    {
        GetComponent<Collider>().enabled = false;
    }
    //-------------------------------------------------
    public virtual void OnBeginDrag(PointerEventData data)
    {
        begin_drag_position = transform.position;

        GameManager.GetInstance().switch_off?.Invoke();
    }

    public virtual void OnDrag(PointerEventData data)
    {
        GameManager GM = GameManager.GetInstance();
        Player owner = card.GetOwner();

        if (GM.GetCurrentGameState() != GameState.None || GM.GetCurrentPlayerTurn() != owner) return;

        Vector3 object_scale = gameObject.transform.localScale;

        // Get the drag vector
        Vector3 drag = Camera.main.ScreenToViewportPoint((Vector3)data.delta);

        // And normalize it so that dragging in any direction is the same.
        drag = new Vector3(drag.x * drag_multiplier_x / object_scale.x, drag.y * drag_multiplier_y / object_scale.y, 0f);

        // Move the card by the drag that we've calculated.
        transform.position = transform.position + drag;

        // And call owner to decrease other card's apha!
        owner.GetHand().change_cards_on_hand_alpha?.Invoke(this, 1f - (begin_drag_position - transform.position).sqrMagnitude / squared_dragged_distance_to_activate_card);
    }

    public virtual void OnEndDrag(PointerEventData data)
    {
        float dragged_distance = (begin_drag_position - transform.position).sqrMagnitude;

        if (data == null)
        {
            dragged_distance = 0f;
        }

        // Based on the dragged distance, decide to put the card back to player's hand or activate it.
        if (dragged_distance >= squared_dragged_distance_to_activate_card)
        {
            card.Activate();
        }
        else
        {
            Move(begin_drag_position, 0.1f);
        }

        card.GetOwner().GetHand().change_cards_on_hand_alpha?.Invoke(this, 1f);

        GameManager.GetInstance().switch_on?.Invoke();
    }
    //----------------------------------------------------------
    public void ChangeInfoAlpha(Visualizer caller, float value)
    {
        if (caller == this) return;

        foreach (Image i in info_images)
        {
            Color c = i.color;
            i.color = new Color(c.r, c.g, c.b, value);
        }

        foreach (TMP_Text t in info_texts)
        {
            Color c = t.color;
            t.color = new Color(c.r, c.g, c.b, value);
        }
    }

    /************************************
    *                                   *
    *     IMOUSEFOCUSHANDLER SECTION    *
    *                                   *
    ************************************/
    public virtual void OnMouseFocus()
    {
        if (!isFocusing)
        {
            non_focus_scale = transform.localScale;

            transform.localScale = new Vector3(focus_scale, focus_scale, 0.01f);
            ReoderCanvas(focus_raise);

            isFocusing = true;
        }
        else
        {
            transform.localScale = non_focus_scale;
            ReoderCanvas(-focus_raise);

            isFocusing = false;
        }
    }

    /************************************
    *                                   *
    *         COROUTINE SECTION         *
    *                                   *
    ************************************/

    protected IEnumerator MoveCard(Vector3 to, float time)
    {
        float elapsed_time = 0f;
        Vector3 old_position = transform.position;
        //Vector3 current_scale = transform.localScale;

        SwitchOff();
        GetComponent<Rigidbody>().isKinematic = true;

        while (elapsed_time < time)
        {
            transform.position = Vector3.Lerp(old_position, to, elapsed_time / time);

            elapsed_time += Time.deltaTime;

            yield return null;
        }


        SwitchOn();
        transform.position = to;
    }

    protected IEnumerator AddCardToHand(Vector3 to, Vector3 sort_point, float show_time, float turn_time, float move_time)
    {
        if (!drew && show_time == 0f) yield break;

        float elapsed_time = 0f;
        SwitchOff();

        if (!drew)
        {
            Quaternion current_angle = transform.localRotation;
            Vector3 start_position = transform.position;
            Vector3 start_scale = transform.localScale;

            while (elapsed_time < show_time / 3f)
            {
                transform.localRotation = Quaternion.Lerp(current_angle, Quaternion.identity, elapsed_time / (show_time / 3f));
                transform.position = Vector3.Lerp(start_position, Vector3.zero, elapsed_time / (show_time / 3f));
                transform.localScale = Vector3.Lerp(start_scale, new Vector3(focus_scale, focus_scale, 0.01f), elapsed_time / (show_time / 3f));

                elapsed_time += Time.deltaTime;
                yield return null;
            }

            transform.localRotation = Quaternion.identity;
            yield return new WaitForSeconds(2f * show_time / 3f);

            elapsed_time = 0f;

            drew = true;
        }

        Vector3 direction = to - sort_point;
        Vector3 current_direction = transform.up;
        while (elapsed_time < turn_time)
        {
            transform.up = Vector3.Lerp(current_direction, direction, elapsed_time / turn_time);

            elapsed_time += Time.deltaTime;
            yield return null;
        }

        transform.up = direction;

        elapsed_time = 0f;
        Vector3 old_position = transform.position;
        Vector3 current_scale = transform.localScale;
        Vector3 new_scale = new Vector3(scale_on_hand, scale_on_hand, 0.01f);

        while (elapsed_time < move_time)
        {
            transform.position = Vector3.Lerp(old_position, to, elapsed_time / move_time);

            Vector3 next_value = Vector3.Lerp(current_scale, new_scale, elapsed_time / (show_time / 3f));

            if (!(float.IsNaN(next_value.x) || float.IsNaN(next_value.y) || float.IsNaN(next_value.z)))
            {
                transform.localScale = next_value;
            }

            elapsed_time += Time.deltaTime;

            yield return null;
        }

        transform.position = to;
        SwitchOn();
    }
}
