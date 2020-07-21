using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleCameraTexture : MonoBehaviour
{
    private bool _camAvailable = false;
    public bool camAvailable => _camAvailable;

    private WebCamTexture cameraTex;

    public MeshRenderer meshRender;

    public AspectRatioFitter fit;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        _camAvailable = false;

        if (devices.Length == 0) {

            Debug.LogWarning("No Camera Available");
            return;
        }

        for (int i = 0; i < devices.Length; i++) { 
            
            if (!devices[i].isFrontFacing)
            {
                cameraTex = new WebCamTexture(devices[i].name, Screen.width, Screen.height);
            }
        }

        if (cameraTex == null) {
            Debug.LogWarning("Find Camera Available");
            return;
        }

        cameraTex.Play();
        meshRender.material.mainTexture = cameraTex;
        _camAvailable = true;
    }

    private void Update()
    {
        if (!camAvailable)
            return;
    }

}
