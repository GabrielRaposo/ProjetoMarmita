using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

class OptionsScreen : MonoBehaviour {
    const float ARBITRARY_MIN = -40, MIN_VOLUME = -80;
    public AudioMixer audioMixer;

    public void SetMasterVolume(float value){
        if (value < ARBITRARY_MIN+1) value = MIN_VOLUME;
        audioMixer.SetFloat("Master", value);
    }
    public void SetBGMVolume(float value){
        if (value < ARBITRARY_MIN + 1) value = MIN_VOLUME;
        audioMixer.SetFloat("BGM", value);
    }
    public void SetUIVolume(float value){
        if (value < ARBITRARY_MIN + 1) value = MIN_VOLUME;
        audioMixer.SetFloat("UI", value);
    }
}

