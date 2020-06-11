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

namespace Storm.Dialog {

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

    private Node _curNode;

    /// <summary>
    /// The current portion of the dialog graph being explored.
    /// </summary>
    public Node CurrentNode {
      get { return _curNode; }
      set {
        Debug.Log("Setting: " + value);
        _curNode = value;
      }
    }

    private string currentSentence;

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

    #region Conversation Begin & End

    /// <summary>
    /// Begins a new dialog with the player.
    /// </summary>
    public void StartDialog() {
      if (!HandlingConversation) {
        Debug.Log("Starting Dialog");
        HandlingConversation = true;
        IsInConversation = true;

        CurrentNode = currentDialog.StartDialog();
        if (CurrentNode is SentenceNode dialog) {
          Debug.Log("Sentence:  " + dialog.text);
        }

        if (CurrentNode == null) {
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
    /// Handles the current node in the appropriate way for any type of dialog node.
    /// </summary>
    public void HandleCurrentNode() {
      switch (CurrentNode) {
        case SentenceNode sentence: {
          WriteSentence(sentence);
          break;
        }

        case TextNode text: {
          WriteText(text.text);
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

    public void WriteSentence(SentenceNode sentence) {
      Debug.Log("Write Sentence.");

      SpeakerText.text = sentence.speaker;
      TypeSentence(sentence.text);
    }

    public void WriteText(string text) {
      SpeakerText.text = "";
      TypeSentence(text);
    }


    public void TypeSentence(string text) {
      if (!HandlingConversation) {
        HandlingConversation = true;

        Debug.Log("Type Sentence");
        if (stillWriting && SentenceText.text != currentSentence) {
          Debug.Log("Skipping Text");
          StopAllCoroutines();
          SentenceText.text = currentSentence;
          TryListDecisions();
        }

        currentSentence = text;

        StopAllCoroutines();
        StartCoroutine(_TypeSentence(currentSentence));
        HandlingConversation = false;
      }
    }

    public void TryListDecisions() {
      var node = CurrentNode.GetOutputPort("output").Connection.node;
      if (node is DecisionNode decisions) {
        ListDecisions(decisions);
      }
      CurrentNode = node;
    }


    public void TakeAction(ActionNode action) {
      if (action.action.GetPersistentEventCount() > 0) {
        action.action.Invoke();
      }

      CurrentNode = action.GetOutputPort("output").Connection.node;
      HandleCurrentNode();
    }

    public void MakeDecision(DecisionNode decisions) {
      int i;
      for (i = 0; i < decisionButtons.Count; i++) {
        if (decisionButtons[i] == EventSystem.current.currentSelectedGameObject) {
          break;
        }
      }

      NodePort outputPort = decisions.GetOutputPort("decisions "+i);
      NodePort inputPort = outputPort.Connection;
      CurrentNode = inputPort.node;

      ClearDecisions();
      HandleCurrentNode();
    }

    /// <summary>
    /// End the current dialog.
    /// </summary>
    public void EndDialog() {
      if (!HandlingConversation) {
        Debug.Log("Closing!");
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

    #region Sentence Handling


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

    #endregion Sentence Handling

    #region Decisions
    /// <summary>
    /// Display a list of decisions a player can make during the conversation.
    /// </summary>
    /// <param name="decisionList">The list of decisions the player can make.</param>
    public void ListDecisions(DecisionNode decisions) {
      List<string> decisionList = decisions.decisions;

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


      Button butt = decisionButtons[0].GetComponent<DecisionBox>().ButtonElement;
      butt.Select();
      butt.interactable = true;
      Debug.Log("decision buttons: "+ decisionButtons.Count);
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
      return CurrentNode is EndDialogNode;
    }

    #endregion



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
  }
}