using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickFeedbackEffect : MonoBehaviour {

    public GameObject effectPrefab;
    public int poolSize = 10;

    List<GameObject> effectPool;
    Vector2 spawnPosition = Vector2.up * 100;
    int currentIndex = 0;

	void Start () {
        effectPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++) {
            GameObject effect = Instantiate(effectPrefab, spawnPosition, Quaternion.identity, transform);
            effectPool.Add(effect);
            effect.SetActive(false);
        }
	}
	
	void Update () {
        if (Input.GetButtonDown("Click")) {
            GameObject effect = GetEffect();
            effect.SetActive(true);
            effect.GetComponent<RectTransform>().position = Input.mousePosition;
            effect.GetComponent<Animator>().Play(0, 0, 0);
        }
	}

    GameObject GetEffect() {
        GameObject effect;
        if(currentIndex < effectPool.Count) {
            effect = effectPool[currentIndex];
            currentIndex = (currentIndex + 1) % effectPool.Count;
            return effect;
        } else {
            return new GameObject();
        }
    }
}
