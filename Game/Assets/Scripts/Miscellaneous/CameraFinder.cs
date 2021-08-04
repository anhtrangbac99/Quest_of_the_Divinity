using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFinder : MonoBehaviour
{
    [SerializeField] private Canvas canvas_renderer = null;

    void Start()
    {
        canvas_renderer.renderMode = RenderMode.WorldSpace;
        canvas_renderer.worldCamera = Camera.main; 
    }
}
