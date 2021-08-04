using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Marker : MonoBehaviour
{
    [SerializeField] private LineRenderer drawer;
    private GameObject start_point;
    private GameObject end_point;
    private bool draw = false;

    public void SetPositions(GameObject start, GameObject end)
    {
        start_point = start;
        end_point = end;

        drawer.enabled = true;
        draw = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (draw)
        {
            if (!end_point.activeInHierarchy)
            {
                draw = false;
                drawer.enabled = false;
            }

            drawer.SetPosition(0, start_point.transform.position);
            drawer.SetPosition(1, end_point.transform.position);
        }
    }

    public bool IsDrawing() => draw;
    public void StopDrawing()
    {
        drawer.enabled = false;
        draw = false;
    }
}
