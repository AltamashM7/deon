using UnityEngine;
using Yarn.Unity;

public class World2_Manager : MonoBehaviour
{
    // Singleton pattern so the door can easily ask the manager if objectives are met
    public static World2_Manager Instance { get; private set; }

    [Header("Objectives")]
    public bool hasTalkedToStranger = false;
    public bool hasTalkedToFriend = false;

    [Header("Setup")]
    [Tooltip("Drag the World 2 Dialogue Runner here")]
    public DialogueRunner dialogueRunner;
    public string introNode = "World2_Intro";

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start()
    {
        // 1. FIX THE BUG: Force the cursor to unlock and become visible for the 2D VN UI!
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (dialogueRunner != null)
        {
            // 2. Wire up the Yarn Command locally (This prevents the World 1 error!)
            dialogueRunner.AddCommandHandler<string>("complete_objective", MarkObjectiveComplete);
            
            // 3. Auto-start the intro after a tiny delay
            Invoke(nameof(StartIntroDialogue), 0.5f);
        }
    }

    private void StartIntroDialogue()
    {
        if (!dialogueRunner.IsDialogueRunning)
        {
            dialogueRunner.StartDialogue(introNode);
        }
    }

    // Notice we removed the [YarnCommand] attribute here and made it an instance method!
    public void MarkObjectiveComplete(string npcName)
    {
        if (npcName.ToLower() == "stranger") hasTalkedToStranger = true;
        if (npcName.ToLower() == "friend") hasTalkedToFriend = true;
    }

    // The door will run this check
    public bool AreAllObjectivesComplete()
    {
        return hasTalkedToStranger && hasTalkedToFriend;
    }
}