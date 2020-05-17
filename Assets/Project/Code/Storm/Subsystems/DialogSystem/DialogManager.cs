using System.Collections;
using System.Collections.Generic;
using Storm.Attributes;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


using Storm.Extensions;
using Storm.Characters.PlayerOld;

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
    /// The current portion of the dialog graph being explored.
    /// </summary>
    private DialogNode currentDialogNode;

    /// <summary>
    /// The current sentence being displayed on screen.
    /// </summary>
    private Sentence currentSentence;

    /// <summary>
    /// The queue of sentences that the DialogManager is currently presenting
    /// .</summary>
    private Queue<Sentence> sentences;

    /// <summary>
    /// The queue of consequences that play out after the most recent player decision.
    /// </summary>
    private Queue<Sentence> consequences;

    #endregion
    
    #endregion

    #region Unity API
    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------

    protected void Start() {

      decisionButtons = new List<GameObject>();
      sentences = new Queue<Sentence>();
      consequences = new Queue<Sentence>();

      var dialogUI = GameObject.FindGameObjectWithTag("DialogUI");
      if (dialogUI != null) {
        DontDestroyOnLoad(dialogUI);
      }
    }

    private void Update() {
      if (IsInConversation && Input.GetKeyDown(KeyCode.Space)) {
        NextSentence();
        if (IsDialogFinished()) {
          var player = GameManager.Instance.player;
          player.NormalMovement.EnableJump();
          player.NormalMovement.EnableMoving();

          // Prevents the player from jumping at
          // the end of every conversation.
          Input.ResetInputAxes();
        }
      } else if (CanStartConversation && Input.GetKeyDown(KeyCode.Space)) {
        RemoveIndicator();
        GameManager.Instance.player.NormalMovement.DisableMoving();
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
        HandlingConversation = true;
        IsInConversation = true;

        if (currentDialog.HasStartEvents()) currentDialog.PerformStartEvents();

        var rootNode = currentDialog.StartDialog();
        if (rootNode != null) {
          SetCurrentNode(currentDialog.StartDialog());
        }


        if (DialogBoxAnim != null) {
          DialogBoxAnim.SetBool("IsOpen", true);
        }

        HandlingConversation = false;
        NextSentence();
      }
    }

    /// <summary>
    /// End the current dialog.
    /// </summary>
    public void EndDialog() {
      if (!HandlingConversation) {
        HandlingConversation = true;

        if (DialogBoxAnim != null) {
          DialogBoxAnim.SetBool("IsOpen", false);
        }

        if (currentDialog.HasCloseEvents()) currentDialog.PerformCloseEvents();

        IsInConversation = false;
        HandlingConversation = false;
      }
    }

    #endregion

    #region Sentence Handling

    /// <summary>
    /// Determine the next sentence to display for the conversation.
    /// </summary>
    public void NextSentence() {
      if (!HandlingConversation) {
        HandlingConversation = true;

        // If you've finished the current dialog node.
        Debug.Log("sentences.Count: " + sentences.Count);
        if (sentences.Count == 0) {

          if (currentDialog.IsFinished()) {
            HandlingConversation = false;
            EndDialog();
            HandlingConversation = true;

          } else {
            NextNode();
            NextSnippet();
          }

        } else {
          NextSnippet();
        }

        HandlingConversation = false;
      }
    }

    /// <summary>
    /// Gets the next node for the conversation. If a player made a decision, 
    /// the next node will be determined from the decision. Otherwise it will
    /// get the next node from the current node, if one is specified.
    /// </summary>
    public void NextNode() {
      Debug.Log("current dialog node: "+currentDialogNode.Name);

      Debug.Log("decision buttons: "+decisionButtons.Count);

      if (decisionButtons.Count > 0) {
        Decision decision = GetDecision();

        ClearDecisions();

        consequences.Clear();
        foreach (Sentence s in decision.Consequences) {
          consequences.Enqueue(s);
        }

        SetCurrentNode(currentDialog.MakeDecision(decision));

      } else if (currentDialogNode.NextNode != null && currentDialogNode.NextNode != "") {
        
        SetCurrentNode(currentDialog.NextNode());
      } else {
        throw new UnityException("Trying to get the next node in a dialog that should have ended!");
      }      
    }

    /// <summary>
    /// Sets the current node of the dialog graph and populates the sentence queue to continue the conversation.
    /// </summary>
    /// <param name="node">The next dialog node to use for the conversation.</param>
    private void SetCurrentNode(DialogNode node) {
      currentDialogNode = node;
      sentences.Clear();
      foreach (Sentence s in currentDialogNode.Sentences) {
        sentences.Enqueue(s);
      }
    }

    /// <summary>
    /// Start typing the next snippet of text onto the screen
    /// </summary>
    public void NextSnippet() {
      if (currentSentence != null && currentSentence.HasEvents()) {
        currentSentence.PerformEvents();
      } else {
        // If the current snippet isn't finished being typed, skip to the end of it.
        if (currentSentence != null && SentenceText.text != currentSentence.SentenceText) {
          StopAllCoroutines();
          SentenceText.text = currentSentence.SentenceText;
          return;
        }

        if (consequences.Count > 0) {
          currentSentence = consequences.Dequeue();
        } else {
          currentSentence = sentences.Dequeue();
        }

        SpeakerText.text = currentSentence.Speaker;

        StopAllCoroutines();
        StartCoroutine(_TypeSentence(currentSentence.SentenceText));
      }
    }

    /// <summary>
    /// A coroutine to type a sentence onto the screen character by character.
    /// </summary>
    /// <param name="sentence">The sentence to type</param>
    IEnumerator _TypeSentence(string sentence) {
      HandlingConversation = true;
      SentenceText.text = "";
      foreach (char c in sentence.ToCharArray()) {
        SentenceText.text += c;
        yield return null;
      }

      if (sentences.Count == 0 &&
        currentDialogNode.Decisions.Count > 0 &&
        !currentDialog.IsFinished()) {

        DisplayDecisions(currentDialogNode.Decisions);
      }

      HandlingConversation = false;
    }

    #endregion Sentence Handling

    #region Decisions
    /// <summary>
    /// Display a list of decisions a player can make during the conversation.
    /// </summary>
    /// <param name="decisionList">The list of decisions the player can make.</param>
    public void DisplayDecisions(List<Decision> decisionList) {

      Debug.Log("Displaying decisions...");
      float buttonHeight = DecisionButtonPrefab.GetComponent<RectTransform>().rect.height;
      float buttonSpace = 0.5f;

      for (int i = 0; i < decisionList.Count; i++) {
        Decision d = decisionList[i];

        // Instantiate button.
        GameObject dButton = Instantiate(
          DecisionButtonPrefab,
          Decisions.transform,
          false
        );

        // Make sure the button's name is unique.
        dButton.name = d.OptionText + " (" + i + ")";

        // Position button and UI anchors.
        RectTransform buttonRect = dButton.GetComponent<RectTransform>();

        buttonRect.anchorMin = new Vector2(0, 1);
        buttonRect.anchorMax = new Vector2(1, 1);

        buttonRect.position -= new Vector3(0, buttonHeight + buttonSpace, 0) * i;

        // Set button properties.
        DecisionBox dBox = dButton.GetComponent<DecisionBox>();
        dBox.SetText(d.OptionText);
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
    /// Get the decision the player made in the current conversation.
    /// </summary>
    /// <returns>The player's decision.</returns>
    public Decision GetDecision() {
      if (decisionButtons.Count == 0) {
        throw new UnityException("Trying to get a dialog decision when no decisions are displayed!");
      }

      // Check which of your buttons are the active selection.
      int i;
      for (i = 0; i < decisionButtons.Count; i++) {
        if (decisionButtons[i] == EventSystem.current.currentSelectedGameObject) {
          break;
        }
      }


      return currentDialogNode.Decisions[i];
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
      currentDialogNode = dialog.GetRootNode();
    }

    public bool IsDialogFinished() {
      Debug.Log("Finished? " + currentDialog.IsFinished());
      return currentDialog.IsFinished();
    }

    #endregion



    /// <summary>
    /// Add the dialog indicator above the player.
    /// </summary>
    public void AddIndicator() {
      PlayerCharacterOld player = GameManager.Instance.player;
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