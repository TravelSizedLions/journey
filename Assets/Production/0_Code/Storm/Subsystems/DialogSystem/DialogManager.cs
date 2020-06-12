using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


using Storm.Extensions;
using Storm.Characters.Player;

using XNode;

namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// A manager for conversations with NPCs and the like. Conversations follow a directed graph pattern.
  /// </summary>
  /// <seealso cref="DialogGraph" />
  public class DialogManager : Singleton<DialogManager> {

    #region Variables
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
    private DialogGraph currentDialog;

    /// <summary>
    /// The current dialog node.
    /// </summary>
    public Node currentNode;

    /// <summary>
    /// The current sentence to be displayed.
    /// </summary>
    private string currentSentence;

    /// <summary>
    /// Whether or not the text is still being written to the screen.
    /// </summary>
    private bool stillWriting;
    #endregion
    #endregion

    #region Unity API
    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------
    protected void Start() {
      decisionButtons = new List<GameObject>();

      var dialogUI = GameObject.FindGameObjectWithTag("DialogUI");
      if (dialogUI != null) {
        DontDestroyOnLoad(dialogUI);
      }

      sentenceTop = SentenceText.rectTransform.offsetMax.y;
    }

    private void Update() {
      if (IsInConversation && Input.GetKeyDown(KeyCode.Space)) {
        HandleCurrentNode();

        if (IsDialogFinished()) {
          // Prevents the player from jumping at
          // the end of every conversation.
          Input.ResetInputAxes();
        }

      } else if (CanStartConversation && Input.GetKeyDown(KeyCode.Space)) {
        RemoveIndicator();
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
        player.DisableMove();
        player.DisableCrouch();
        StartDialog();
      }
    }
    #endregion

    #region Public Interface
    //---------------------------------------------------------------------
    // Public Interface
    //---------------------------------------------------------------------

    #region Top-Level 
    /// <summary>
    /// Handles the current node in the appropriate way for any type of dialog node.
    /// </summary>
    public void HandleCurrentNode() {
      switch (currentNode) {
        case SentenceNode sentence: {
          WriteSentence(sentence);
          break;
        }

        case TextNode text: {
          WriteText(text.Text);
          break;
        }

        case ActionNode action: {
          TakeAction(action);
          break;
        }

        case DecisionNode decisions: {
          MakeDecision(decisions);
          break;
        }

        case EndDialogNode end: {
          EndDialog();
          break;
        }
      }
    }
    #endregion

    #region Sentence Handling
    /// <summary>
    /// Write out a sentence with a speaker.
    /// </summary>
    /// <param name="sentence">The sentence node.</param>
    public void WriteSentence(SentenceNode sentence) {
      if (SpeakerText.text == "") {
        Debug.Log("Move down");
        SentenceText.rectTransform.offsetMax = new Vector2(
          SentenceText.rectTransform.offsetMax.x, 
          sentenceTop
        );
      }
      SpeakerText.text = sentence.Speaker;
      TypeSentence(sentence.Text);
    }

    /// <summary>
    /// Write out a sentence without a speaker.
    /// </summary>
    /// <param name="text">the text to write.</param>
    public void WriteText(string text) {
      if (SpeakerText.text != "") {
        Debug.Log("Move up");
        SpeakerText.text = "";
        SentenceText.rectTransform.offsetMax = new Vector2(
          SentenceText.rectTransform.offsetMax.x, 
          0
        );
      }
      TypeSentence(text);
    }

    /// <summary>
    /// Start typing out the sentence.
    /// </summary>
    /// <param name="text">The text to write.</param>
    public void TypeSentence(string text) {
      if (!HandlingConversation) {
        HandlingConversation = true;

        if (stillWriting && SentenceText.text != currentSentence) {

          StopAllCoroutines();
          SentenceText.text = currentSentence;
          TryListDecisions();

        } else {

          currentSentence = text;
          StopAllCoroutines();
          StartCoroutine(_TypeSentence(currentSentence));

        }

        HandlingConversation = false;
      }
    }

    /// <summary>
    /// A coroutine to type a sentence onto the screen character by character.
    /// </summary>
    /// <param name="sentence">The sentence to type</param>
    IEnumerator _TypeSentence(string sentence) {
      HandlingConversation = true;
      stillWriting = true;
      SentenceText.text = "";

      foreach (char c in sentence.ToCharArray()) {
        SentenceText.text += c;
        yield return null;
      }
 
      Debug.Log("Finished normally");
      TryListDecisions();

      stillWriting = false;
      HandlingConversation = false;
    }
    #endregion

    #region UnityEvent Handling

    /// <summary>
    /// Perform a UnityEvent between sentences.
    /// </summary>
    /// <param name="action">The action node.</param>
    public void TakeAction(ActionNode action) {
      if (action.Action.GetPersistentEventCount() > 0) {
        action.Action.Invoke();
      }

      currentNode = action.GetOutputPort("Output").Connection.node;
      HandleCurrentNode();
    }
    #endregion

    #region Conversation Terminals
    /// <summary>
    /// Begins a new dialog with the player.
    /// </summary>
    public void StartDialog() {
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
        HandleCurrentNode();
      }
    }

    /// <summary>
    /// End the current dialog.
    /// </summary>
    public void EndDialog() {
      if (!HandlingConversation) {
        PlayerCharacter player = FindObjectOfType<PlayerCharacter>();
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

    #region Decision Making
    /// <summary>
    /// See if there's a list of decisions to display.
    /// </summary>
    public void TryListDecisions() {
      var node = currentNode.GetOutputPort("Output").Connection.node;
      if (node is DecisionNode decisions) {
        ListDecisions(decisions);
      }
      currentNode = node;
    }

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
    /// Get the player's selected decision.
    /// </summary>
    /// <param name="decisions">The decision node.</param>
    public void MakeDecision(DecisionNode decisions) {
      int i;
      for (i = 0; i < decisionButtons.Count; i++) {
        if (decisionButtons[i] == EventSystem.current.currentSelectedGameObject) {
          break;
        }
      }

      NodePort outputPort = decisions.GetOutputPort("Decisions "+i);
      NodePort inputPort = outputPort.Connection;
      currentNode = inputPort.node;

      decisions.SetPreviousDecision(i);

      ClearDecisions();
      HandleCurrentNode();
    }


    /// <summary>
    /// Clear the list of possible decisions from the screen.
    /// </summary>
    public void ClearDecisions() {
      for (int i = 0; i < decisionButtons.Count; i++) {
        Destroy(decisionButtons[i]);
      }

      decisionButtons.Clear();
    }

    #endregion Decisions

    #region Getters & Setters
    public void SetCurrentDialog(DialogGraph dialog) {
      currentDialog = dialog;
    }

    public bool IsDialogFinished() {
      return currentNode is EndDialogNode;
    }

    #endregion

    #region Player Indication
    /// <summary>
    /// Add the dialog indicator above the player.
    /// </summary>
    public void AddIndicator() {
      PlayerCharacter player = FindObjectOfType<PlayerCharacter>();

      indicatorInstance = Instantiate<GameObject>(
        IndicatorPrefab,
        player.transform.position + IndicatorPosition,
        Quaternion.identity
      );

      indicatorInstance.transform.parent = player.transform;
      CanStartConversation = true;
    }

    /// <summary>
    /// Remove the dialog indicator from the player.
    /// </summary>
    public void RemoveIndicator() {
      if (indicatorInstance != null) {
        Destroy(indicatorInstance.gameObject);
      }
      CanStartConversation = false;
    }
    #endregion
    #endregion
  }
}