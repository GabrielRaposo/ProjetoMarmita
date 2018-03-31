using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using Ink.Runtime;
using RedBlueGames.Tools.TextTyper;

public class InkManager : MonoBehaviour {

    [Header("Ink Assets")]
    public TextAsset inkJSONAsset;

    [Header("UI References")]
    public TextMeshProUGUI dialogBox;
    public GameObject dialogPanel;
    public GameObject proceedArrow;
    public RectTransform buttonsCanvas;
    public Button clickArea;

    [Header("Manager References")]
    public CharacterManager characterPortrait;
    public BackgroundManager backgroundManager;

    [Header("Prefabs & Values")]
    public TextMeshProUGUI textPrefab;
    public Button buttonPrefab;
    public Color dialogTextColor;
    public Color narrationTextColor;
    [Range(0f, 1f)] public float textSpeed = .5f;


    [Header("Audio References")]
    public AudioSource typeSound;
    public AudioSource beepSound;
    public AudioSource confirmSound;

    public enum State { Narration, Dialog, Choice }
    public State state;

    Story story;
    TextTyper dialogTyper;
    static public InkManager instance;

    private void Awake() {
        if (!instance)
            instance = this;
        else
            Destroy(gameObject);
        dialogTyper = dialogBox.GetComponent<TextTyper>();

        StartStory();
    }

    void SkipText() {
        if (dialogTyper && dialogTyper.IsSkippable())
            dialogTyper.Skip();
    }

    private void StartStory() {
        story = new Story(inkJSONAsset.text);
        SetExternalFunction();

        InterpretStory();
    }

    private void SetExternalFunction() {
        story.BindExternalFunction("change_background", (string TAG) => ChangeBackground(TAG));
    }

    private void InterpretStory() {
        ClearButtons();
        SetDialogState();

        //float typeSpeed = 1f / (textSpeed * 10);
        float typeSpeed = (2.5001f - (textSpeed * 2.5f))/80;
        log("typeSpeed: " + typeSpeed);


        if (story.canContinue) {
            string section = story.Continue();
            if(IsDialog(section)) {
                for(int i = 0; i < section.Length; i++) {
                    if(section[i] == ']') section = section.Substring(i + 1).TrimStart();
                }
                SetDialogState();
            } else {
                SetNarrationState();
            }
            dialogTyper.TypeText(section, typeSpeed);
            dialogTyper.PrintCompleted.RemoveAllListeners();
            dialogTyper.PrintCompleted.AddListener(() => StartCoroutine(DialogHalt()));
            dialogTyper.CharacterPrinted.AddListener((string str) => PlayTypeSound());

            clickArea.onClick.AddListener(() => SkipText());
        }
    }

    /* private void InterpretStory()
    {
        ClearButtons();

        float typeSpeed = 1f / (textSpeed * 10);
        if (story.canContinue)
        {
            string section = story.Continue();
            if (IsDialog(section))
            {
                for (int i = 0; i < section.Length; i++)
                {
                    if (section[i] == ']') section = section.Substring(i + 1).TrimStart();
                }

                SetDialogState();
                dialogTyper.TypeText(section, typeSpeed);
                dialogTyper.PrintCompleted.RemoveAllListeners();
                dialogTyper.PrintCompleted.AddListener(() => StartCoroutine(DialogLoop()));
                dialogTyper.CharacterPrinted.AddListener((string str) => PlayTypeSound());
            }
            else
            {
                SetNarrationState();

                TextMeshProUGUI storyText = Instantiate(textPrefab) as TextMeshProUGUI;
                storyText.transform.SetParent(narrationPanel.transform, false);
                narrationTyper = storyText.GetComponent<TextTyper>();

                narrationTyper.TypeText(section, typeSpeed);
                narrationTyper.PrintCompleted.RemoveAllListeners();
                narrationTyper.PrintCompleted.AddListener(() => StartCoroutine(NarrationLoop()));
                narrationTyper.CharacterPrinted.AddListener((string str) => PlayTypeSound());
            }
        }
    }
    */

    //se retornar algo igual, é porque mudou nada e não é tag
    private bool IsDialog(string section){
        string tag = string.Empty;
        section = section.Trim();
        if(section[0] == '[') {
            int index = 0;
            while (section[index] != ']') {
                index++;
            }
            tag = section.Remove(index + 1);
            log("tag: " + tag);
        }
        char[] removeList = { '[' , ']' };
        characterPortrait.SetPortrait(tag.Trim(removeList));
        return !(tag == string.Empty);
    }

    private void SetNarrationState() {
        dialogBox.color = narrationTextColor;
        characterPortrait.gameObject.SetActive(false);
        dialogPanel.SetActive(true);
        state = State.Narration;
    }

    private void SetDialogState() {
        dialogBox.color = dialogTextColor;
        characterPortrait.gameObject.SetActive(true);
        dialogPanel.SetActive(true);
        state = State.Dialog;
    }

    private void SetChoiceState() {
        characterPortrait.gameObject.SetActive(false);
        dialogPanel.SetActive(false);
        state = State.Choice;
    }

    IEnumerator DialogHalt() {
        clickArea.onClick.RemoveAllListeners();
        for (int i = 0; i < 3; i++) yield return new WaitForEndOfFrame();
        proceedArrow.SetActive(true);
        clickArea.onClick.AddListener(() => DialogLoop());
    }

    void DialogLoop() {
        clickArea.onClick.RemoveAllListeners();
        beepSound.Play();
        proceedArrow.SetActive(false);
        if (story.canContinue) {
            InterpretStory();
        } else {
            CallButtons();
        }
    }

    private void CallButtons() {
        SetChoiceState();
        if (story.currentChoices.Count > 0) {
            for (int i = 0; i < story.currentChoices.Count; i++) {
                Choice choice = story.currentChoices[i];
                Button button = CreateChoiceView(choice.text.Trim());
                button.onClick.AddListener(delegate
                {
                    OnClickChoiceButton(choice);
                });
            }
        } else {
            log("call End of Story.");
            Button choice = CreateChoiceView("End of story. Restart?");
            choice.onClick.AddListener(delegate { StartStory(); });
        }
    }

    private void PlayTypeSound() {
        if(!typeSound.isPlaying) typeSound.Play();
    }

    Button CreateChoiceView (string text) {
		Button choice = Instantiate (buttonPrefab) as Button;
		choice.transform.SetParent (buttonsCanvas.transform, false);

        TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
		choiceText.text = text;

		return choice;
	}

    void OnClickChoiceButton (Choice choice) {
		story.ChooseChoiceIndex (choice.index);
        if (confirmSound) confirmSound.Play();
        InterpretStory();
	}

    void ClearButtons() {
        int childCount = buttonsCanvas.transform.childCount;
        for (int i = childCount - 1; i >= 0; --i) {
            GameObject.Destroy(buttonsCanvas.transform.GetChild(i).gameObject);
        }
    }

    void ChangeBackground(int ID) {
        Debug.Log("ID: " + ID);
        backgroundManager.SetBG(ID);
    }

    void ChangeBackground(string tag) {
        Debug.Log("tag: " + tag);
        backgroundManager.SetBG(tag);
    }

    public void SetPause(bool value) {
        if (value) {
            SkipText();
        } else {

        }
        clickArea.gameObject.SetActive(!value);
    }

    public void SetTextSpeed(float value) {
        textSpeed = value; 
    }

    void log(string str) { Debug.Log(str); }
}
