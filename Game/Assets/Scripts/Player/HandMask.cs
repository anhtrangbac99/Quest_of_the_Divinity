using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandMask : MonoBehaviour, IMouseFocusHandler
{
    private bool is_focusing = false;
    private Vector3 starting_position;
    private Vector3 starting_box_size;
    // Start is called before the first frame update
    void Start()
    {
        starting_position = transform.position;
        starting_box_size = GetComponent<BoxCollider>().size;
    }

    public void OnMouseFocus()
    {
        if (!is_focusing)
        {
            Vector3 new_position = new Vector3(0f, transform.position.y, 0f);

            if (new_position.y > 0f)
            {
                new_position -= 2f * Vector3.up;
            }
            else
            {
                new_position += 2f * Vector3.up;
            }

            transform.position = new_position;

            is_focusing = true;
        }
        else
        {
            transform.position = starting_position;
            is_focusing = false;
        }
        
    }
}
