using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour {

    Image image;

    [System.Serializable]
    public struct BackgroundInfo {
        public string tag;
        public Sprite image;
    }
    public BackgroundInfo[] bgsInfo;
    public int current { get; private set; }

    private void Start() {
        image = GetComponent<Image>();
    }

    public void SetBG(int index) {
        image = GetComponent<Image>();
        index %= bgsInfo.Length;
        if(index == 0) {
            image.color = Color.black;
        } else {
            image.color = Color.white;
            image.sprite = bgsInfo[index].image;
        }
        current = index;
    }
}
