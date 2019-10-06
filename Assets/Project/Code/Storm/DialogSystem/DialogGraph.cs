using System;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;
using UnityEngine.Events;

using Storm.Characters.Player;

namespace Storm.DialogSystem {
    [Serializable]
    public class DialogGraph : MonoBehaviour {

        public bool isSpacePressed;

        // The first set of dialog in a conversation.
        private DialogNode root;

        // The current dialog node.
        private DialogNode current;

        // TODO: Make a way to import/export to XML
        public string file;

        public float startEventsDelay;
        public UnityEvent startEvents;

        

        // The GRAPH of the conversation
        // (why do developers exclusively refer to this as a tree?)
        public DialogNode[] nodes;
        
        private Dictionary<string, DialogNode> graph;

        public float closeEventsDelay;
        public UnityEvent closeEvents;

        





        //---------------------------------------------------------------------
        // Constructor(s)
        //---------------------------------------------------------------------
        public void Awake() {
            if (file == "")  {
                graph = new Dictionary<string, DialogNode>();
                foreach (DialogNode n in nodes) {
                    graph.Add(n.key, n);
                }

                if (nodes.Length > 0) {
                    root = nodes[0];
                }
            }
        }


        public void Update() {
            isSpacePressed = isSpacePressed || Input.GetKeyDown(KeyCode.Space);
        }




        //---------------------------------------------------------------------
        // Graph Building
        //---------------------------------------------------------------------

        public bool AddDialog(DialogNode node) {
            if (graph.Count == 0) {
                root = node;
            }
            graph[node.key] = node;
            return true;
        }

        // Add a transition from one Dialog node to another.
        public void AddTransition(string fromTag, 
                                  string optionText, 
                                  string toTag) {
                                      
            DialogNode fromNode = graph[fromTag];
            fromNode.AddDecision(optionText, toTag);
        }

        public void Clear() {
            graph.Clear();
        }




        //---------------------------------------------------------------------
        // Graph Traversal
        //---------------------------------------------------------------------

        public DialogNode GetRootNode() {
            return root;
        }

        public DialogNode StartDialog() {
            current = root;
            PerformStartEvents();
            return root;
        }

        public DialogNode MakeDecision(Decision decision) {
            current = graph[decision.destinationTag];
            return current;
        }

        public DialogNode GetCurrentDialog() {
            return current;
        }

        public bool IsFinished() {
            return current.decisions.Count == 0;
        }




        //---------------------------------------------------------------------
        // Event Handling
        //---------------------------------------------------------------------
        public bool HasStartEvents() {
            return (startEvents.GetPersistentEventCount() > 0);
        }

        public void PerformStartEvents() {
            startEvents.Invoke();
        }

        public bool HasCloseEvents() {
            return (closeEvents.GetPersistentEventCount() > 0);
        }

        public void PerformCloseEvents() {
            closeEvents.Invoke();
        }



        //---------------------------------------------------------------------
        // Dialog Triggering
        //---------------------------------------------------------------------

        public void OnTriggerEnter2D(Collider2D other) {

            // If the player is in the trigger area
            if (other.CompareTag("Player")) {
                PlayerCharacter player = GameManager.Instance.player;
                player.activeMovementMode.DisableJump();
                InGameDialogManager.Instance.AddIndicator();
                InGameDialogManager.Instance.SetCurrentDialog(this);
            }
        }


        public void OnTriggerExit2D(Collider2D other) {
            // If the player has left the trigger area
            if (other.CompareTag("Player") && !InGameDialogManager.Instance.isInConversation) {
                PlayerCharacter player = GameManager.Instance.player;
                player.activeMovementMode.EnableJump();
                InGameDialogManager.Instance.RemoveIndicator();
            }
        }


    }
}
