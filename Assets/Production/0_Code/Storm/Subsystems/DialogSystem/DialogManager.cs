using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


using Storm.Extensions;
using Storm.Characters.Player;
using Storm.Subsystems.Transitions;

using XNode;
using UnityEngine.SceneManagement;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A manager for conversations with NPCs and the like. Conversations follow a directed graph pattern.
  /// </summary>
  /// <seealso cref="DialogGraph" />
  public class DialogManager : Singleton<DialogManager> {

    #region Variables
    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private IPlayer player;

    #region Display Elements
    [Header("Display Elements", order = 0)]
    [Space(5, order = 1)]

    /// <summary>
    /// The UI element to use in displaying the speaker's name.
    /// </summary>
    [Tooltip("The UI element to use in displaying the current speaker's name.")]
    public TextMeshProUGUI SpeakerText;

    /// <summary>
    /// The UI element to use in displaying the conversation.
    /// </summary>
    [Tooltip("The UI element to use in displaying the conversation.")]
    public TextMeshProUGUI SentenceText;

    /// <summary>
    /// The position of the top of the sentence text box when the dialog has a speaker.
    /// </summary>
    private float sentenceTop;

    /// <summary>
    /// The RectTransform used as a parent for decision buttons.
    /// </summary>
    [Tooltip("The RectTransform used as a parent for decision buttons.")]
    public RectTransform Decisions;

    /// <summary>
    /// The UI prefab used to represent a decision the player can make.
    /// </summary>
    [Tooltip("The UI prefab used to represent a decision the player can make.")]
    public GameObject DecisionButtonPrefab;

    /// <summary>
    /// The UI representation of the decisions the player can make.
    /// </summary>
    private List<GameObject> decisionButtons;

    /// <summary>
    /// The animator used to open and close the dialog box.
    /// </summary>
    [Tooltip("The animator used to open and close the dialog box.")]
    public Animator DialogBoxAnim;


    [Space(10, order = 2)]
    #endregion

    #region Dialog Indication
    [Header("Dialog Indication", order = 3)]
    [Space(5, order = 4)]

    /// <summary>
    /// The prefab used to indicate that the player can start a conversation.
    /// </summary>
    [Tooltip("The prefab used to indicate that the player can start a conversation.")]
    public GameObject IndicatorPrefab;

    /// <summary>
    /// The actual instance of the dialog indicator.
    /// </summary>
    private GameObject indicatorInstance;

    /// <summary>
    /// The position of the dialog indicator relative to the player.
    /// </summary>
    [Tooltip("The position of the dialog indicator relative to the player.")]
    public Vector3 IndicatorPosition;

    [Space(10, order=5)]
    #endregion

    #region Management Flags
    [Header("Conversation State Management", order = 6)]
    [Space(5, order = 7)]

    /// <summary>
    /// Whether or not the player can start a conversation.
    /// </summary>
    [Tooltip("Whether or not the player can start a conversation.")]
    [SerializeField]
    [ReadOnly]
    public bool CanStartConversation;

    /// <summary>
    /// Whether or not the player is currently in a conversation.
    /// </summary>
    [Tooltip("Whether or not the player is currently in a conversation.")]
    [ReadOnly]
    public bool IsInConversation;

    /// <summary>
    /// Whether or not the manager is currently busy managing the conversation.
    /// </summary>
    [Tooltip("Whether or not the manager is currently busy managing the conversation.")]
    [ReadOnly]
    public bool HandlingConversation;

    #endregion

    #region Dialog Graph Model

    /// <summary>
    /// The current conversation being played out.
    /// </summary>
    private IDialog currentDialog;

    /// <summary>
    /// The current dialog node.
    /// </summary>
    public IDialogNode currentNode;

    /// <summary>
    /// Whether or not the text is still being written to the screen.
    /// </summary>
    public bool StillWriting;
    #endregion
    #endregion

    #region Unity API
    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------
    protected void Start() {
      player = FindObjectOfType<PlayerCharacter>();
      decisionButtons = new List<GameObject>();

      SceneManager.sceneLoaded += OnNewScene;

      var dialogUI = GameObject.FindGameObjectWithTag("DialogUI");
      if (dialogUI != null) {
        DontDestroyOnLoad(dialogUI);
      }

      sentenceTop = SentenceText.rectTransform.offsetMax.y;
    }

    /// <summary>
    /// Dependency injection point for a reference to the player.
    /// </summary>
    /// <param name="player">A reference to the player.</param>
    public void Inject(IPlayer player) {
      this.player = player;
    }

    /// <summary>
    /// Dependency injection point for a Dialog graph.
    /// </summary>
    /// <param name="dialog">The dialog to inject</param>
    public void Inject(IDialog dialog) {
      this.currentDialog = dialog;
    }

    /// <summary>
    /// Dependency injection point for a dialog node.
    /// </summary>
    /// <param name="node">The node to inject.</param>
    public void Inject(IDialogNode node) {
      this.currentNode = node;
    }

    #endregion

    #region Public Interface
    //---------------------------------------------------------------------
    // Public Interface
    //---------------------------------------------------------------------

    #region Top-Level Interface
    /// <summary>
    /// Begins a new dialog with the player.
    /// </summary>
    public void StartDialog(IDialog graph) {
      Debug.Log("Start Dialog!");

      player.DisableJump();
      player.DisableMove();
      player.DisableCrouch();

      SetCurrentDialog(graph);

      if (!HandlingConversation) {
        HandlingConversation = true;
        IsInConversation = true;

        currentNode = currentDialog.StartDialog();

        if (currentNode == null) {
          return;
        }

        if (DialogBoxAnim != null) {
          DialogBoxAnim.SetBool("IsOpen", true);
        }

        HandlingConversation = false;
        ContinueDialog();
      }
    }

    /// <summary>
    /// Continues the dialog.
    /// </summary>
    public void ContinueDialog() {
      currentNode.HandleNode();
    }

    
    /// <summary>
    /// End the current dialog.
    /// </summary>
    public void EndDialog() {
      if (!HandlingConversation) {
        player.EnableJump();
        player.EnableCrouch();
        player.EnableMove();

        HandlingConversation = true;

        if (DialogBoxAnim != null) {
          DialogBoxAnim.SetBool("IsOpen", false);
        }

        IsInConversation = false;
        HandlingConversation = false;
      }
    }
    #endregion

    #region Sentence Handling
    public void SkipTyping(string text) {
      StopAllCoroutines();
      if (SentenceText != null) {
        SentenceText.text = text;
      }
    }

    public void StopTyping() {
      StopAllCoroutines();
    }

    public void StartTyping(IEnumerator typingRoutine) {
      StartCoroutine(typingRoutine);
    }

    /// <summary>
    /// Clear the text that's displayed on screen.
    /// </summary>
    public void ClearText() {
      if (SentenceText != null) {
        SentenceText.text = "";
      }
    }

    /// <summary>
    /// Type a single character onto the screen.
    /// </summary>
    /// <param name="c">The character to type.</param>
    public void Type(char c) {
      SentenceText.text += c;
    }

    /// <summary>
    /// Whether or not the dialog has finished typing the given text.
    /// </summary>
    /// <param name="targetText"></param>
    /// <returns></returns>
    public bool IsFinishedTyping(string targetText) {
      return SentenceText.text == targetText;
    }
    #endregion

    #region Decision Making
    /// <summary>
    /// Display a list of decisions a player can make during the conversation.
    /// </summary>
    /// <param name="decisionList">The list of decisions the player can make.</param>
    public void ListDecisions(DecisionNode decisions) {
      List<string> decisionList = decisions.Decisions;

      float buttonHeight = DecisionButtonPrefab.GetComponent<RectTransform>().rect.height;
      float buttonSpace = 0.5f;

      for (int i = 0; i < decisionList.Count; i++) {
        string text = decisionList[i];

        // Instantiate button.
        GameObject dButton = Instantiate(
          DecisionButtonPrefab,
          Decisions.transform,
          false
        );

        // Make sure the button's name is unique.
        dButton.name = text + " (" + i + ")";

        // Position button and UI anchors.
        RectTransform buttonRect = dButton.GetComponent<RectTransform>();

        buttonRect.anchorMin = new Vector2(0, 1);
        buttonRect.anchorMax = new Vector2(1, 1);

        buttonRect.position -= new Vector3(0, buttonHeight + buttonSpace, 0) * i;

        // Set button properties.
        DecisionBox dBox = dButton.GetComponent<DecisionBox>();
        dBox.SetText(text);
        dBox.SetDecision(i);

        // Add to list of decisions.
        decisionButtons.Add(dButton);
      }


      int prevDecisionIndex = decisions.GetPreviousDecision();
      Button butt = decisionButtons[prevDecisionIndex].GetComponent<DecisionBox>().ButtonElement;
      butt.Select();
      butt.interactable = true;
    }

    
    /// <summary>
    /// Get the on screen decision buttons.
    /// </summary>
    /// <returns>The list of decision buttons on screen.</returns>
    public List<GameObject> GetDecisionButtons() {
      return decisionButtons;
    }

    /// <summary>
    /// Remove the decision buttons from the screen.
    /// </summary>
    public void ClearDecisions() {
      for (int i = 0; i < decisionButtons.Count; i++) {
        Destroy(decisionButtons[i]);
      }

      decisionButtons.Clear();
    }

    #endregion

    #region Getters & Setters

    public void SetCurrentNode(IDialogNode node) {
      currentNode = node;
    }

    public void SetCurrentDialog(IDialog dialog) {
      currentDialog = dialog;
    }

    public bool IsDialogFinished() {
      return currentNode == null;
    }

    public void SetSpeakerText(string speaker) {
      if (speaker != null && SpeakerText != null) {

        if (speaker != "" & SpeakerText.text == "") {
          SentenceText.rectTransform.offsetMax = new Vector2(
            SentenceText.rectTransform.offsetMax.x, 
            sentenceTop
          );
        } else if (speaker == "" && SpeakerText.text != "") {
          SpeakerText.text = "";
          SentenceText.rectTransform.offsetMax = new Vector2(
            SentenceText.rectTransform.offsetMax.x, 
            0
          );
        }

        SpeakerText.text = speaker;
      }
    }

    #endregion
    #endregion
  
    private void OnNewScene(Scene aScene, LoadSceneMode aMode) {
      player = FindObjectOfType<PlayerCharacter>();
    }
  }
}