using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimpleVideoControlBarCtrl : MonoBehaviour
{

    [SerializeField]
    Button play_pause_btn;

    [SerializeField]
    Button setting_btn;

    [SerializeField]
    Image play_pause_image;

    [SerializeField]
    Slider transition_slider;

    [SerializeField]
    Transform transition_panel;

    [SerializeField]
    Sprite playSprite;

    [SerializeField]
    Sprite pauseSprite;

    private bool isTransitionPanelOpen = false;

    public void SetUp(System.Action<bool> videoCalback, System.Action<float> transitionCallback) {


        RegisterPlayPauseEvent(videoCalback);
        RegisterTransitonEvent(transitionCallback);

        transition_slider.value = 0.9f;
    }

    private void RegisterPlayPauseEvent(System.Action<bool> videoCalback) {
        play_pause_btn.onClick.RemoveAllListeners();

        play_pause_image.sprite = pauseSprite;

        play_pause_btn.onClick.AddListener(() =>
        {
            play_pause_image.sprite = play_pause_image.sprite.name == playSprite.name ? pauseSprite : playSprite;

            videoCalback(play_pause_image.sprite.name != playSprite.name);
        });
    }

    private void RegisterTransitonEvent(System.Action<float> transitionCallback) {
        transition_panel.gameObject.SetActive(false);
        setting_btn.onClick.RemoveAllListeners();
        setting_btn.onClick.AddListener(() => {
            isTransitionPanelOpen = !isTransitionPanelOpen;

            transition_panel.gameObject.SetActive(isTransitionPanelOpen);
        });

        transition_slider.onValueChanged.AddListener((float num) =>
        {
            transitionCallback(num);
        });
    }

}
