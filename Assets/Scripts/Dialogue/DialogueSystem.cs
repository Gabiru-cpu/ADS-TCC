using UnityEngine;

public enum STATE
{
    DISABLED,
    WAITING,
    TYPING
}


public class DialogueSystem : MonoBehaviour
{

    // public DialogueData dialogueData;
    private DialogueData currentDialogueData;


    int currentText = 0;
    public bool finished = false;
    public Xingu xingu;

    TypeTextAnimation typeText;
    DialogueUI dialogueUI;    

    public STATE state;

    public STATE State { get; internal set; }

    void Awake()
    {

        typeText = FindObjectOfType<TypeTextAnimation>();
        dialogueUI = FindObjectOfType<DialogueUI>();

        typeText.TypeFinished = OnTypeFinishe;

    }

    void Start()
    {
        state = STATE.DISABLED;
    }

    void Update()
    {

        if (state == STATE.DISABLED) return;

        switch (state)
        {
            case STATE.WAITING:
                Waiting();
                break;
            case STATE.TYPING:
                Typing();
                break;
        }

    }

    public void SetDialogueData(DialogueData data)
    {
        currentDialogueData = data;
    }

    public void Next()
    {
        if (currentText == 0)
        {
            dialogueUI.Enable();
        }

        dialogueUI.SetName(currentDialogueData.talkScript[currentText].name);

        typeText.fullText = currentDialogueData.talkScript[currentText++].text;

        if (currentText == currentDialogueData.talkScript.Count) finished = true;

        typeText.StartTyping();
        state = STATE.TYPING;

    }

    void OnTypeFinishe()
    {
        state = STATE.WAITING;
    }

    void Waiting()
    {

        if (Input.GetKeyDown(KeyCode.E))
        {

            if (!finished)
            {
                xingu.talk = true;
                Next();
            }
            else
            {
                xingu.talk = false;
                End();
            }

        }

    }

    public void End()
    {
        dialogueUI.Disable();
        state = STATE.DISABLED;
        currentText = 0;
        finished = false;
    }

    void Typing()
    {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            typeText.Skip();
            state = STATE.WAITING;
        }

    }

}