using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class Camera1D : Behaviour1D
{
    public int resolution = 30;

    private Camera _camera;

    private int _lastWidth;
    private int _lastHeight;

    void OnEnable()
    {
        _camera = GetComponent<Camera>();

        Vector3 pos = transform.position;
        pos.z = -1;
        transform.position = pos;

        if (_camera != null)
        {
            Setup();
        }
    }

    private void Update()
    {
        if (Screen.width != _lastWidth ||
            Screen.height != _lastHeight)
        {
            Setup();
        }
    }

    void Setup()
    {
        _lastWidth = Screen.width;
        _lastHeight = Screen.height;

        int zoomedHeight = _lastHeight * resolution / _lastWidth;
        float pixelSizeY = 1.0f / zoomedHeight;

        _camera.rect = new Rect(0, 0.5f - pixelSizeY * 0.5f, 1.0f, pixelSizeY);
        _camera.orthographic = true;
        _camera.orthographicSize = 0.5f;
    }
}
