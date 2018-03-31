﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterManager : MonoBehaviour {

    public Image portraitImage;
    public TextMeshProUGUI nameBox;

	[System.Serializable]
    public struct CharacterInfo {
        public string tag;
        public string name;
        public Sprite image;
    }
    public CharacterInfo[] charactersInfo;
    public CharacterInfo current { get; private set; }

    public void SetPortrait(string tag) {
        CharacterInfo info = charactersInfo[0];
        foreach (CharacterInfo ci in charactersInfo) {
            if (ci.tag == tag) {
                info = ci;
                break;
            }
        }
        current = info;
        SetInfo(info);
    }

    private void SetInfo(CharacterInfo info) {
        nameBox.text = info.name;
        portraitImage.sprite = info.image;
    }
}