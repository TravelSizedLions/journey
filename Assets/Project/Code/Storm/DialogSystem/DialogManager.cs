using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using TMPro;

using Storm.Attributes;

namespace Storm.DialogSystem {

    /// <summary>
    /// A manager for conversations with NPC's and the like. Handles any type of 
    /// graph configuration, including cycles.
    /// </summary>
    public class DialogManager : MonoBehaviour {

        #region Display Elements
        [Header("Display Elements", order=0)]
        [Space(5, order=1)]

        /// <summary>The UI element to use in displaying the speaker's name.</summary>
        [Tooltip("The UI element to use in displaying the current speaker's name.")]
        public TextMeshProUGUI speakerText;

        /// <summary>The UI element to use in displaying the conversation.</summary>
        [Tooltip("The UI element to use in displaying the conversation.")]
        public TextMeshProUGUI sentenceText;

        /// <summary>The RectTransform used as a parent for decision buttons.</summary>
        [Tooltip("The RectTransform used as a parent for decision buttons.")]
        public RectTransform decisions;

        /// <summary>
        /// The UI prefab used to represent a decision the player can make.
        /// </summary>
        public GameObject decisionButtonPrefab;

        /// <summary>
        /// The UI representation of the decisions the player can make.
        /// </summary>
        public List<GameObject> decisionButtons;


        /// <summary>The animator used to open and close the dialog box.</summary>
        [Tooltip("The animator used to open and close the dialog box.")]
        public Animator dialogBoxAnim;

        [Space(15,order=2)]
        #endregion

        #region Management Flags
        [Header("Management Flags", order=3)]
        [Space(5, order=4)]

        /// <summary>Whether or not the player can start a conversation.</summary>
        [Tooltip("Whether or not the player can start a conversation.")]
        [ReadOnly]
        public bool canStartConversation;

        /// <summary>Whether or not the player is currently in a conversation.</summary>
        [Tooltip("Whether or not the player is currently in a conversation.")]
        [ReadOnly]
        public bool isInConversation;

        /// <summary>Whether or not the manager is currently busy managing the conversation.</summary>
        [Tooltip("Whether or not the manager is currently busy managing the conversation.")]
        [ReadOnly]
        public bool handlingConversation;

        #endregion

        #region Dialog Graph Model

        /// <summary>The current conversation being played out.</summary>
        private DialogGraph currentDialog;

        /// <summary>The current portion of the dialog graph being explored.</summary>
        private DialogNode currentDialogNode;

        /// <summary>The current sentence being displayed on screen. </summary>
        private Sentence currentSnippet;

        /// <summary>The queue of sentences that the DialogManager is currently presenting.</summary>
        private Queue<Sentence> snippets;

        /// <summary>The queue of consequences that play out after the most recent player decision.</summary>
        private Queue<Sentence> consequences;

        #endregion

        #region Unity API
        //---------------------------------------------------------------------
        // Unity API
        //---------------------------------------------------------------------

        public void Awake() {
            snippets  = new Queue<Sentence>();
            consequences = new Queue<Sentence>();
            
            var dialogUI = GameObject.FindGameObjectWithTag("DialogUI");
            if (dialogUI != null) {
                DontDestroyOnLoad(dialogUI);
            }
        }

        #endregion

        #region Dialog Handling
        //---------------------------------------------------------------------
        // Dialog Handling Functions
        //---------------------------------------------------------------------
        
        #region Conversation Begin & End

        /// <summary>
        /// Begins a new dialog.
        /// </summary>
        public void StartDialog() {
            if (!handlingConversation) {
                handlingConversation = true;
                isInConversation = true;

                if (currentDialog.HasStartEvents()) currentDialog.PerformStartEvents();

                var rootNode = currentDialog.StartDialog();
                if (rootNode != null) {
                    SetCurrentNode(currentDialog.StartDialog());
                }
                

                if (dialogBoxAnim != null) {
                    dialogBoxAnim.SetBool("IsOpen", true);
                }

                handlingConversation = false;
                NextSentence();
            }
        }

        /// <summary>
        /// End the current dialog.
        /// </summary>
        public void EndDialog() {
            if (!handlingConversation) {
                handlingConversation = true;

                if (dialogBoxAnim != null) {
                    dialogBoxAnim.SetBool("IsOpen", false);
                }
                
                if (currentDialog.HasCloseEvents()) currentDialog.PerformCloseEvents();

                isInConversation = false;
                handlingConversation = false;
            }
        }

        #endregion

        #region Sentence Handling

        /// <summary>
        /// Determine the next sentence to display for the conversation.
        /// </summary>
        public void NextSentence() {
            if (!handlingConversation) {
                handlingConversation = true;

                if (snippets.Count == 0) {

                    if (currentDialog.IsFinished()) {
                        handlingConversation = false;
                        EndDialog();
                        handlingConversation = true;

                    } else {
                        
                        NextNode();

                        if (currentSnippet != null && currentSnippet.HasEvents()) {
                            currentSnippet.PerformEvents();
                        } else {
                            NextSnippet();
                        }
                        
                    }

                } else {

                    if (currentSnippet != null && currentSnippet.HasEvents()) {
                        currentSnippet.PerformEvents();
                    } else {
                        NextSnippet();
                    }
                }

                handlingConversation = false;
            }
        }

        /// <summary>
        /// Gets the next node for the conversation. If a player made a decision, 
        /// the next node will be determined from the decision. Otherwise it will
        /// get the next node from the current node, if one is specified.
        /// </summary>
        public void NextNode() {
            if (currentDialogNode.decisions.Count > 0) {
                
                Decision decision = GetDecision();

                ClearDecisions();

                consequences.Clear();
                foreach (Sentence s in decision.consequences) {
                    consequences.Enqueue(s);
                }

                SetCurrentNode(currentDialog.MakeDecision(decision));
            } else if (currentDialogNode.nextNode != null && currentDialogNode.nextNode != "") {
                SetCurrentNode(currentDialog.GetNode(currentDialogNode.nextNode));
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
            snippets.Clear();
            foreach(Sentence s in currentDialogNode.snippets) {
                snippets.Enqueue(s);
            }
        }

        /// <summary>
        /// Start typing the next snippet of text onto the screen
        /// </summary>
        public void NextSnippet() {
            if (currentSnippet != null && sentenceText.text != currentSnippet.sentence) {
                StopAllCoroutines();
                sentenceText.text = currentSnippet.sentence;
                return;
            }

            if (consequences.Count > 0) {
                currentSnippet = consequences.Dequeue();
            } else {
                currentSnippet = snippets.Dequeue();
            }
        
            speakerText.text = currentSnippet.speaker;

            StopAllCoroutines();        
            StartCoroutine(_TypeSentence(currentSnippet.sentence));
        }

        /// <summary>
        /// A coroutine to type a sentence onto the screen character by character.
        /// </summary>
        /// <param name="sentence">The sentence to type</param>
        IEnumerator _TypeSentence (string sentence) {
            handlingConversation = true;
            sentenceText.text = "";
            foreach(char c in sentence.ToCharArray()) {
                sentenceText.text += c;
                yield return null;
            }

            if (snippets.Count == 0 && 
                currentDialogNode.decisions.Count > 0 &&
                !currentDialog.IsFinished()) {

                DisplayDecisions(currentDialogNode.decisions);
            }

            handlingConversation = false;
        }

        #endregion Sentence Handling

        #region Decisions
        /// <summary>
        /// Display a list of decisions a player can make during the conversation.
        /// </summary>
        /// <param name="decisionList">The list of decisions the player can make.</param>
        public void DisplayDecisions(List<Decision> decisionList) {

            float buttonHeight = decisionButtonPrefab.GetComponent<RectTransform>().rect.height;
            float buttonSpace = 0.5f;

            for (int i = 0; i < decisionList.Count; i++) {
                Decision d = decisionList[i];

                // Instantiate button.
                GameObject dButton = Instantiate(
                    decisionButtonPrefab,
                    decisions.transform,
                    false
                );

                // Make sure the button's name is unique.
                dButton.name = d.optionText + " ("+i+")";

                // Position button and UI anchors.
                RectTransform buttonRect = dButton.GetComponent<RectTransform>();

                buttonRect.anchorMin = new Vector2(0,1);
                buttonRect.anchorMax = new Vector2(1,1);

                buttonRect.position -= new Vector3(0,buttonHeight+buttonSpace,0)*i;

                // Set button properties.
                DecisionBox dBox = dButton.GetComponent<DecisionBox>();
                dBox.SetText(d.optionText);
                dBox.SetDecision(i);

                // Add to list of decisions.
                decisionButtons.Add(dButton);
            }


            Button butt = decisionButtons[0].GetComponent<DecisionBox>().button;
            butt.Select();
            butt.interactable = true;

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
            for (i=0; i < decisionButtons.Count; i++) {
                if (decisionButtons[i] == EventSystem.current.currentSelectedGameObject) {
                    break;
                }
            }


            return currentDialogNode.decisions[i];
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
            return currentDialog.IsFinished();
        }

        #endregion

        #endregion
    }
}
