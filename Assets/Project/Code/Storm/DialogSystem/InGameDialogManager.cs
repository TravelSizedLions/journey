using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using TMPro;

using Storm.Extensions;
using Storm.Characters.Player;

namespace Storm.DialogSystem {

    [RequireComponent(typeof(DialogManager))]
    public class InGameDialogManager : Singleton<InGameDialogManager> {

        public DialogManager manager;

        public GameObject indicatorPrefab;

        private GameObject indicatorInstance;

        public Vector3 indicatorPosition;

        public bool isInConversation {
            get { return manager.isInConversation; }
        }


        #region Unity Functions
        //---------------------------------------------------------------------
        // Unity Functions
        //---------------------------------------------------------------------
        public override void Awake() {
            base.Awake();
            //DontDestroyOnLoad(GameObject.FindGameObjectWithTag("UI"));
            manager = GetComponent<DialogManager>();
        }

        public void Update() {
           if (manager.isInConversation && Input.GetKeyDown(KeyCode.Space)) {
                manager.NextSentence();
                if (manager.IsDialogFinished()) {
                    var player = GameManager.Instance.player;
                    player.activeMovementMode.EnableJump();
                    player.activeMovementMode.EnableMoving();
                    
                    // Prevents the player from jumping at
                    // the end of every conversation.
                    Input.ResetInputAxes();
                }
            } else if (manager.canStartConversation && Input.GetKeyDown(KeyCode.Space)) {
                RemoveIndicator();
                GameManager.Instance.player.activeMovementMode.DisableMoving();
                manager.StartDialog();
            }
        }

        #endregion

        #region Dialog Handling
        //---------------------------------------------------------------------
        // Dialog Handling Functions
        //---------------------------------------------------------------------

        #endregion

        #region Getters / Setters
        public void SetCurrentDialog(DialogGraph dialog) {
            manager.SetCurrentDialog(dialog);
        }

        #endregion

        #region Indicator Functions

        public void AddIndicator() {
            PlayerCharacter player = GameManager.Instance.player;
            indicatorInstance = Instantiate<GameObject>(
                indicatorPrefab, 
                player.transform.position+indicatorPosition, 
                Quaternion.identity
            );

            indicatorInstance.transform.parent = player.transform;
            manager.canStartConversation = true;
        }

        public void RemoveIndicator() {
            if (indicatorInstance != null) {
                    Destroy(indicatorInstance.gameObject);
            }
            manager.canStartConversation = false;
        }

        #endregion
    }
}
