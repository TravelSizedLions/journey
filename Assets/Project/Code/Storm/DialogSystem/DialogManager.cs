using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

using Storm.Extensions;
using Storm.Characters.Player;

namespace Storm.DialogSystem {
    public class DialogManager : MonoBehaviour {
        public bool canStartConversation;
        public bool isInConversation;
        public bool handlingConversation;

        public TextMeshProUGUI speakerText;
        public TextMeshProUGUI sentenceText;

        public Animator dialogBoxAnim;
        public Queue<Sentence> snippets;
        public Queue<Sentence> consequences;

        private DialogGraph currentDialog;

        private DialogNode currentDialogNode;

        private Sentence currentSnippet;

        public GameObject indicatorPrefab;

        private GameObject indicatorInstance;

        public Vector3 indicatorPosition;


        #region Unity Functions
        //---------------------------------------------------------------------
        // Unity Functions
        //---------------------------------------------------------------------

        public void Awake() {
            snippets  = new Queue<Sentence>();
            consequences = new Queue<Sentence>();
        }

        #endregion

        #region Dialog Handling
        //---------------------------------------------------------------------
        // Dialog Handling Functions
        //---------------------------------------------------------------------
        // Begins a new dialog.
        public void StartDialog() {
            if (!handlingConversation) {
                _StartDialog();
            }
        }

        // Begin dialog Co-Routine.
        private void _StartDialog() {
            handlingConversation = true;
            isInConversation = true;

            if (currentDialog.HasStartEvents()) currentDialog.PerformStartEvents();

            var rootNode = currentDialog.StartDialog();
            if (rootNode != null) {
                SetCurrentNode(currentDialog.StartDialog());
            } else {
                Debug.Log("NODE IS EMPTY!");
            }
            

            if (dialogBoxAnim != null) {
                dialogBoxAnim.SetBool("IsOpen", true);
            }

            handlingConversation = false;
            NextSentence();    
        }

        private void SetCurrentNode(DialogNode node) {
            currentDialogNode = node;
            snippets.Clear();
            foreach(Sentence s in currentDialogNode.snippets) {
                snippets.Enqueue(s);
            }
        }

        //TODO: Add logic for decisions
        public void NextNode() {
            if (currentDialogNode.decisions.Count > 0) {
                Decision decision = currentDialogNode.decisions[0];
                consequences.Clear();
                foreach (Sentence s in decision.consequences) {
                    consequences.Enqueue(s);
                }
                SetCurrentNode(currentDialog.MakeDecision(decision));
            }
        }


        // Continues a dialog.
        public void NextSentence() {
            if (!handlingConversation) {
                _NextSentence();
            }
        }

        // Continue dialog Co-Routine.
        private void _NextSentence() {
            handlingConversation = true;

            if (snippets.Count == 0) {
                if (currentDialog.IsFinished()) {
                    handlingConversation = false;
                    EndDialog();
                    handlingConversation = true;

                } else {
                    // TODO: Add logic for decisions
                    
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

        public bool PerformSnippetEvents() {
            if (currentSnippet.HasEvents()) {
                currentSnippet.PerformEvents();
                return true;
            }
            return false;
        }

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

        IEnumerator _TypeSentence (string sentence) {
            handlingConversation = true;
            sentenceText.text = "";
            foreach(char c in sentence.ToCharArray()) {
                sentenceText.text += c;
                yield return null;
            }
            handlingConversation = false;
        }


        // Ending a dialog.
        public void EndDialog() {
            if (!handlingConversation) {
                _EndDialog();
            }
        }


        // 
        private void _EndDialog() {
            handlingConversation = true;

            if (dialogBoxAnim != null) {
                dialogBoxAnim.SetBool("IsOpen", false);
            }
            
            if (currentDialog.HasCloseEvents()) currentDialog.PerformCloseEvents();

            isInConversation = false;
            handlingConversation = false;
        }

        #endregion

        #region Getters / Setters
        public void SetCurrentDialog(DialogGraph dialog) {
            currentDialog = dialog;
        }

        public bool IsDialogFinished() {
            return currentDialog.IsFinished();
        }

        #endregion
    }
}
