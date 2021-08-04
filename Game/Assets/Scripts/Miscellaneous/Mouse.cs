using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mouse : MonoBehaviour
{
    public Texture2D cursor_default_texture;
    public Texture2D cursor_attack_texture;
    public CursorMode cursor_mode = CursorMode.Auto;
    public Vector2 hot_spot = Vector2.zero;

    public LayerMask raycast_layer;
    public LayerMask hand_layer;
    public LayerMask on_click_raycast_layer;
    public RaycastHit last_info;

    private Collider hand;
    private static Mouse instance = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        hand = null;
        ChangeState(MouseState.Default);
    }

    void Update()
    {
        if (!(Input.GetMouseButton(0)
        || Input.GetMouseButton(1)))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 50, hand_layer))
            {
                if (hand == null || !hand.Equals(info.collider))
                {
                    hand?.GetComponent<IMouseFocusHandler>().OnMouseFocus();
                    info.collider.GetComponent<IMouseFocusHandler>().OnMouseFocus();
                    hand = info.collider;
                }
            }
            else if (hand)
            {
                hand.GetComponent<IMouseFocusHandler>().OnMouseFocus();
                hand = null;
            }

            if (hand != null && Physics.Raycast(ray, out info, 50, raycast_layer))
            {
                if (!last_info.Equals(info))
                {
                    last_info.collider?.gameObject.GetComponent<IMouseFocusHandler>().OnMouseFocus();

                    info.collider.gameObject.GetComponent<IMouseFocusHandler>().OnMouseFocus();
                }
            }
            else
            {
                last_info.collider?.gameObject.GetComponent<IMouseFocusHandler>().OnMouseFocus();
            }

            last_info = info;
        }
        else if (Input.GetMouseButton(1))
        {
            GameManager.GetInstance().NotifyAttackIsOver(true);
        }
        else if (Input.GetMouseButton(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 50, on_click_raycast_layer))
            {
                info.collider.gameObject.GetComponent<IMouseDownHandler>()?.OnMouseClickDown();
            }
        }
    }

    public static Mouse GetInstance() => instance;

    public void ChangeState(MouseState state)
    {
        Texture2D texture = cursor_default_texture;
        switch (state)
        {
            case MouseState.Attack:
                texture = cursor_attack_texture;
                break;
        }

        Cursor.SetCursor(texture, hot_spot, cursor_mode);
    }

    public void ResetHand()
    {
        hand = null;
    }
    // private IEnumerator DelayOnHand()
    // {
    //     cast_on_hand = false;

    //     yield return new WaitForSeconds(0.25f);

    //     cast_on_hand = true;
    // }
}
