using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundManager : MonoBehaviour {
    Image image;
    public BackgroundInfo[] bgsInfo;
    public BackgroundInfo current { get; private set; }

    public void SetBG(string tag) {
        image = GetComponent<Image>();
        BackgroundInfo info = bgsInfo[0];
        foreach(BackgroundInfo bi in bgsInfo) {
            if(bi.tag == tag) {
                info = bi;
                break;
            }
        }
        SetValues(info);
    }

    public void SetBG(int index) {
        image = GetComponent<Image>();
        index %= bgsInfo.Length;

        SetValues(bgsInfo[index]);
    }

    public void SetValues(BackgroundInfo info) {
        image.color = info.color;
        image.sprite = info.sprite;
        current = info;
    }
}
