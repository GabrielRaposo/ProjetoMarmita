using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour {

    public void StartGame() {
        //Scenemanager.LoadSceneAsync(1)...
        SceneManager.LoadScene(1);
    }
}
