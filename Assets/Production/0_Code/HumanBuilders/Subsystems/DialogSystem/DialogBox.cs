using System.Collections;
using System.Collections.Generic;
using HumanBuilders.Graphing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HumanBuilders {

  /// <summary>
  /// A class representing a Dialog Box UI element.
  /// </summary>
  public class DialogBox : MonoBehaviour, IDialogBox {
    //---------------------------------------------------------------------
    // Properties
    //---------------------------------------------------------------------

    [Header("UI Elements", order=0)]
    [Space(10, order=1)]

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
    /// The list of elements that are colored with the primary color.
    /// </summary>
    [Tooltip("The list of elements that are colored with the primary color.")]
    public List<Image> PrimaryColorComponents;

    /// <summary>
    /// The list of elements that are colored with the secondary color.
    /// </summary>
    [Tooltip("The list of elements that are colored with the secondary color.")]
    public List<Image> SecondaryColorComponents;

    [Header("Decision Making", order=2)]
    [Space(10, order=3)]

    private Color DefaultTextColor;

    private Color DefaultPrimaryColor;

    private Color DefaultSecondaryColor;

    /// <summary>
    /// The RectTransform used as a parent for decision buttons.
    /// </summary>
    [Tooltip("The RectTransform used as a parent for decision buttons.")]
    public RectTransform Decisions;

    /// <summary>
    /// The UI prefab used to represent a decision the player can make.
    /// </summary>
    [Tooltip("The UI prefab used to represent a decision the player can make.")]
    public DecisionBox DecisionButtonPrefab;

    [Tooltip("The sound to use while speaking.")]
    public AudioSource TalkingSound;

    /// <summary>
    /// The animator used to open and close the dialog box.
    /// </summary>
    private Animator animator;

    /// <summary>
    /// The position of the top of the sentence text box when the dialog has a speaker.
    /// </summary>
    private float sentenceTop;

    /// <summary>
    /// The UI representation of the decisions the player can make.
    /// </summary>
    private List<DecisionBox> decisionButtons;

    /// <summary>
    /// An instance of the dialog manager.
    /// </summary>
    private DialogManager manager; 

    /// <summary>
    /// The co-routine that types out characters onto the dialog box.
    /// </summary>
    private Coroutine typingCoroutine;

    private CharacterProfile currentCharacter; 


    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------
    private void Awake() {
      animator = GetComponent<Animator>();
      manager = DialogManager.Instance;
      decisionButtons = new List<DecisionBox>();

      if (PrimaryColorComponents.Count > 0) {
        DefaultPrimaryColor = PrimaryColorComponents[0].color;
      }

      if (SecondaryColorComponents.Count > 0) {
        DefaultSecondaryColor = SecondaryColorComponents[0].color;
      }
      
      if (SentenceText != null) {
        sentenceTop = SentenceText.rectTransform.offsetMax.y;
        DefaultTextColor = SentenceText.color;
      }
    }

    //---------------------------------------------------------------------
    // Public Interface
    //---------------------------------------------------------------------
    /// <summary>
    /// Open the dialog box.
    /// </summary>
    public void Open() {
      animator.SetBool("IsOpen", true);
    }

    /// <summary>
    /// Close the dialog box.
    /// </summary>
    public void Close() {
      animator.SetBool("IsOpen", false);
      TalkingSound.Stop();
      ApplyColors(DefaultPrimaryColor, DefaultSecondaryColor, DefaultTextColor);
    }

    /// <summary>
    /// Type out a sentence spoken by a certain speaker. If another sentence is
    /// already being typed, the dialog box will instead skip to the end of the
    /// sentence being typed. If the player will need to make a decision
    /// afterward, this will also display the options.
    /// </summary>
    /// <param name="message">The message to type.</param>
    public void Type(Message message) => Type(message.Sentence, message.Speaker, false, 0, message.Speed);

    /// <summary>
    /// Type out a sentence spoken by a certain speaker. If another sentence is
    /// already being typed, the dialog box will instead skip to the end of the
    /// sentence being typed. If the player will need to make a decision
    /// afterward, this will also display the options.
    /// </summary>
    /// <param name="sentence">The sentence to type.</param>
    /// <param name="speaker">The speaker of the sentence.</param>
    /// <param name="autoAdvance">Whether or not to automatically advance the
    /// dialog after typing has finished.</param>
    /// <param name="delayBeforeAdvance">How long to delay before advancing the
    /// dialog, in seconds</param>
    /// <param name="typingSpeed">The speed of typing, in characters per second.</param>
    /// <param name="animateSpeaker">Whether or not to control the speaker's animation.</param>
    public void Type(string sentence, string speaker = "", bool autoAdvance = false, float delayBeforeAdvance = 0f, float typingSpeed = 100f, bool animateSpeaker = true) {
      SetSpeakerText(speaker);

      if (manager.StillWriting && !IsFinishedTyping(sentence)) {
        // Stop typing, just display the whole thing and wait for the next input.
        SkipTyping(sentence);
        TryListDecisions();
        if (animateSpeaker) {
          DialogManager.StopTalking(currentCharacter);
        }
      } else {
        // Start typing out the next sentence.
        if (typingCoroutine != null) {
          StopCoroutine(typingCoroutine);
        }        
        typingCoroutine = StartCoroutine(_TypeSentence(sentence, autoAdvance, delayBeforeAdvance, typingSpeed, animateSpeaker));
      }
    }

    /// <summary>
    /// Clears the sentence text on the dialog box.
    /// </summary>
    public void ClearText() {
      if (SentenceText != null) {
        SentenceText.text = "";
      }
    }

    /// <summary>
    /// Remove the decision buttons from the screen.
    /// </summary>
    public void ClearDecisions() {
      for (int i = 0; i < decisionButtons.Count; i++) {
        Destroy(decisionButtons[i].gameObject);
      }

      decisionButtons.Clear();
    }

    /// <summary>
    /// Gets the list of active decisions on screen.
    /// </summary>
    public List<DecisionBox> GetDecisionButtons() {
      return decisionButtons;
    }

    public void SetCurrentCharacter(CharacterProfile character) {
      // TODO: may need to find a place to make sure this is unset/reset to some
      // default...
      currentCharacter = character;
      ApplyColors(character);
    }

    public void ApplyColors(CharacterProfile profile) {
      ApplyColors(
        profile.PrimaryColor,
        profile.SecondaryColor,
        profile.TextColor
      );
    }

    /// <summary>
    /// Apply character profile information to this dialog box.
    /// </summary>
    /// <param name="profile">The profile to apply.</param>
    public void ApplyColors(Color primary, Color secondary, Color text) {
      ApplyPrimaryColor(primary);
      ApplySecondaryColor(secondary);
      ApplyTextColor(text);
    }

    private void ApplyPrimaryColor(Color color) {
      foreach (Image image in PrimaryColorComponents) {
        image.color = color;
      }
    }

    private void ApplySecondaryColor(Color color) {
      foreach (Image image in SecondaryColorComponents) {
        image.color = color;
      }
    }

    private void ApplyTextColor(Color color) {
      SpeakerText.color = color;
      SentenceText.color = color;
    }

    public void ResetColors() {
      ApplyColors(DefaultPrimaryColor, DefaultSecondaryColor, DefaultTextColor);
    }

    //---------------------------------------------------------------------
    // Helper Methods
    //---------------------------------------------------------------------

    /// <summary>
    /// Whether or not the dialog has finished typing the given text.
    /// </summary>
    /// <param name="targetText">The final typed text.</param>
    /// <returns>Whether or not all of the given text has been typed.</returns>
    private bool IsFinishedTyping(string targetText) {
      return SentenceText.text == targetText;
    }


    /// <summary>
    /// Set the speaker text that's displayed to the user. 
    /// If there is no speaker (speaker == ""), Then the UI will collapse that part of the Dialog Box.
    /// </summary>
    private void SetSpeakerText(string speaker) {
      if (speaker != null && SpeakerText != null) {
        if (speaker != "") {
          SentenceText.rectTransform.offsetMax = new Vector2(
            SentenceText.rectTransform.offsetMax.x, 
            sentenceTop
          );
        } else if (speaker == "") {
          SentenceText.text = "";
          SentenceText.rectTransform.offsetMax = new Vector2(
            SentenceText.rectTransform.offsetMax.x, 
            0
          );
        }

        SpeakerText.text = speaker;
      }
    }

    //---------------------------------------------------------------------
    // Typing
    //---------------------------------------------------------------------

    /// <summary>
    /// The coroutine that handles typing out the sentence.
    /// </summary>
    /// <param name="sentence">The sentence to type.</param>
    /// <param name="autoAdvance">Whether or not to automatically advance the
    /// dialog after typing has finished.</param>
    /// <param name="delayBeforeAdvance">How long to delay before advancing the
    /// dialog, in seconds</param>
    /// <param name="speed">The speed of typing, in characters per second.</param>
    private IEnumerator _TypeSentence(string sentence, bool autoAdvance, float delayBeforeAdvance, float speed, bool animateSpeaker) {
      manager.StillWriting = true;
      TalkingSound.Play();

      if (animateSpeaker) {
        DialogManager.StartTalking(currentCharacter);
      }

      ClearText();

      float waitTime = 1/speed;

      if (sentence != null) {
        for (int i = 0; i < sentence.Length; i++) {
          char c = sentence[i];
          TypeCharacter(c);

          // Waiting for any time after the sentence is finished opens the door
          // for a race condition.
          if (i < sentence.Length-1) {
            if (DialogManager.Punctuation.ContainsKey(c)) {
              TalkingSound.Stop();
              yield return new WaitForSeconds(DialogManager.Punctuation[c]);
              TalkingSound.Play();
            } else {
              yield return new WaitForSeconds(waitTime);
            }
          }
        }
      }

      TryListDecisions();

      if (animateSpeaker) {
        DialogManager.StopTalking(currentCharacter);
      }
      
      TalkingSound.Stop();
      manager.StillWriting = false;

      if (delayBeforeAdvance > 0) {
        yield return new WaitForSeconds(delayBeforeAdvance);
      }

      if (autoAdvance) {
        DialogManager.ContinueDialog();
      }
    }

    /// <summary>
    /// Type a single character onto the screen.
    /// </summary>
    /// <param name="c">The character to type.</param>
    private void TypeCharacter(char c) {
      SentenceText.text += c;
    }

    
    /// <summary>
    /// Stop typing the current sentence and just display it in full.
    /// </summary>
    private void SkipTyping(string text) {
      if (typingCoroutine != null) {
        StopCoroutine(typingCoroutine);
      }
      
      if (SentenceText != null) {
        SentenceText.text = text;
      }

      TalkingSound.Stop();
      manager.StillWriting = false;
    }

    //---------------------------------------------------------------------
    // Decision Making
    //---------------------------------------------------------------------

    /// <summary>
    /// If the next node is a decision node, then this will display those
    /// decisions to the user.
    /// </summary>
    private void TryListDecisions() {
      var node = DialogManager.GetCurrentNode();
      if (node != null) {
        node = node.GetNextNode();

        if (node is DecisionNode decisions) {
          ListDecisions(decisions);
          DialogManager.SetCurrentNode(node);
        } else {
          DialogManager.SetCurrentNode(node, false);
        }
      }
    }


    /// <summary>
    /// Display a list of decisions a player can make during the conversation.
    /// </summary>
    /// <param name="decisionList">The list of decisions the player can make.</param>
    private void ListDecisions(DecisionNode decisions) {
      List<string> decisionList = decisions.Decisions;

      float buttonHeight = DecisionButtonPrefab.GetComponent<RectTransform>().rect.height;

      for (int i = 0; i < decisionList.Count; i++) {
        string text = decisionList[i];

        // Instantiate button.
        DecisionBox dButton = Instantiate<DecisionBox>(
          DecisionButtonPrefab,
          Decisions.transform,
          false
        );

        // Set the buttons to the same color as the box.
        if (PrimaryColorComponents.Count > 0 &&
            SecondaryColorComponents.Count > 0 &&
            SpeakerText != null) {

          dButton.ApplyColors(
            PrimaryColorComponents[0].color,
            SecondaryColorComponents[0].color,
            SpeakerText.color
          );
        }

        // Make sure the button's name is unique.
        dButton.name = text + "_" + i;

        // Set button properties.
        DecisionBox dBox = dButton.GetComponent<DecisionBox>();
        dBox.SetText(text);
        dBox.SetDecision(i);

        // Add to list of decisions.
        decisionButtons.Add(dButton);
      }


      int prevDecisionIndex = decisions.GetPreviousDecision();
      Button butt = decisionButtons[prevDecisionIndex].GetComponent<DecisionBox>().ButtonElement;
      butt.interactable = true;
    }

    private void OnDestroy() {
      TalkingSound?.Stop();
    }
  }
}