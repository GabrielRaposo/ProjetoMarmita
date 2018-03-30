using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Ink.Runtime;
using RedBlueGames.Tools.TextTyper;

public class InkManager : MonoBehaviour {

    public TextAsset inkJSONAsset;
    public TextMeshProUGUI narrationBox, dialogBox;
    public TextTyper narrationTyper;
    public Button buttonPrefab;
    public RectTransform buttonsCanvas;

    private Story story;
    private bool textIsRunning;

    //typewrite stuff
    [Range(1f, 10f)] public float textSpeed = 5;
    [Range(1f, 10f)] public float commaDelayMultiplier = 5;
    [Range(1f, 10f)] public float ponctuationDelayMultiplier = 5;

    AudioSource typeSound;

    enum State { Narration, Dialog }

    private void Awake() {
        typeSound = GetComponent<AudioSource>();

        StartStory();
    }

    private void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (narrationTyper.IsSkippable()) narrationTyper.Skip();
        }
    }

    private void StartStory() {
        story = new Story(inkJSONAsset.text);
        SetExternalFunction();

        CallStorytelling();
    }

    private void SetExternalFunction() {
        story.BindExternalFunction("change_background", (int ID) => ChangeBackground(ID));
    }

    private void ClearTexts() {
        if (narrationBox) narrationBox.text = string.Empty;
        if (dialogBox) dialogBox.text = string.Empty;
    }

    private void CallStorytelling() {
        ClearTexts();
        ClearButtons();

        string text = string.Empty;
        while (story.canContinue) {
            string section = story.Continue();
            //checa se é dialogo
            text += section;
        }
        narrationTyper.TypeText(text, 1f / (textSpeed*10));
        narrationTyper.PrintCompleted.RemoveAllListeners();
        narrationTyper.PrintCompleted.AddListener(() => StartCoroutine(SetText()));
        narrationTyper.CharacterPrinted.AddListener((string str) => PlayTypeSound());
    }

    private void PlayTypeSound() {
        if(!typeSound.isPlaying) typeSound.Play();
    }

    private void CallButtons() {
        if (story.currentChoices.Count > 0) {
            for (int i = 0; i < story.currentChoices.Count; i++)
            {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                button.onClick.AddListener(delegate
                {
                    OnClickChoiceButton(choice);
                });
            }
        } else {
            log("call End of Story.");
            Button choice = CreateChoiceView("End of story.\nRestart?");
            choice.onClick.AddListener(delegate { StartStory(); });
        }
    }

    IEnumerator SetText() {
        if (story.canContinue) {
            yield return new WaitForSeconds(1); 
            CallStorytelling();
        } else {
            CallButtons();
        }
    }

    Button CreateChoiceView (string text) {
		Button choice = Instantiate (buttonPrefab) as Button;
		choice.transform.SetParent (buttonsCanvas.transform, false);

        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
		choiceText.text = text;

		HorizontalLayoutGroup layoutGroup = choice.GetComponent <HorizontalLayoutGroup> ();
		layoutGroup.childForceExpandHeight = false;

		return choice;
	}

    void OnClickChoiceButton (Choice choice) {
		story.ChooseChoiceIndex (choice.index);
        CallStorytelling();
	}

    void ClearButtons() {
        int childCount = buttonsCanvas.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i) {
            GameObject.Destroy(buttonsCanvas.transform.GetChild(i).gameObject);
        }
    }

    void SetCharacter(int ID) {

    }

    void ChangeBackground(int ID) {
        Debug.Log("ID: " + ID);
    }

    void log(string str) { Debug.Log(str); }
}
