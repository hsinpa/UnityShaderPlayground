using RenderHeads.Media.AVProVideo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleVideoCtrl : MonoBehaviour
{
    [SerializeField]
    private SimpleVideoControlBarCtrl control_bar_view;

    [SerializeField]
    private MeshRenderer videoDisplayRenderer;

    [SerializeField]
    private MediaPlayer mediaPlayer;

    Material videoDisplayMat;

    private void Start()
    {

        videoDisplayMat = videoDisplayRenderer.material;

        control_bar_view.SetUp((bool isPlay) => {

            if (isPlay)
                OnVideoPlay();
            else
                OnVideoPause();

        }, OnVideoDisplayTransitionChange);

        
    }

    private void OnVideoPlay() {
        mediaPlayer.Play();
    }

    private void OnVideoPause()
    {
        mediaPlayer.Pause();
    }

    private void OnVideoDisplayTransitionChange(float t_value) {
        videoDisplayMat.SetFloat("_Transition", t_value);
    }

}
