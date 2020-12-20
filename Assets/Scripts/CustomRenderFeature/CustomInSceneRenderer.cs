using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class CustomInSceneRenderer : MonoBehaviour
{
    [SerializeField]
    private MyBlitFeature myBlitFeature;

    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
