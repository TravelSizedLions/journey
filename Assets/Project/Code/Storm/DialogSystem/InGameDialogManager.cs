using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Storm.Characters.Player;
using Storm.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Storm.DialogSystem {

  [RequireComponent(typeof(DialogManager))]
  public class InGameDialogManager : Singleton<InGameDialogManager> {

    #region Dialog Manager
    [Header("Dialog Manager", order = 0)]
    [Space(5, order = 1)]

    /// <summary> The underlying dialog manager. </summary>
    [Tooltip("The underlying dialog manager.")]
    public DialogManager manager;

    [Space(15, order = 2)]
    #endregion

    #region Dialog Indication
    [Header("Dialog Indication", order = 3)]
    [Space(5, order = 4)]

    /// <summary>The prefab used to indicate that the player can start a conversation.</summary>
    [Tooltip("The prefab used to indicate that the player can start a conversation.")]
    public GameObject indicatorPrefab;

    /// <summary>The actual instance of the dialog indicator.</summary>
    private GameObject indicatorInstance;

    /// <summary> The position of the dialog indicator relative to the player.</summary>
    [Tooltip("The position of the dialog indicator relative to the player.")]
    public Vector3 indicatorPosition;
    #endregion

    public bool isInConversation {
      get { return manager.isInConversation; }
    }


    #region Unity Functions
    //---------------------------------------------------------------------
    // Unity Functions
    //---------------------------------------------------------------------
    protected override void Awake() {
      base.Awake();
      manager = GetComponent<DialogManager>();
    }

    public void Update() {
      if (manager.isInConversation && Input.GetKeyDown(KeyCode.Space)) {
        manager.NextSentence();
        if (manager.IsDialogFinished()) {
          var player = GameManager.Instance.player;
          player.NormalMovement.EnableJump();
          player.NormalMovement.EnableMoving();

          // Prevents the player from jumping at
          // the end of every conversation.
          Input.ResetInputAxes();
        }
      } else if (manager.canStartConversation && Input.GetKeyDown(KeyCode.Space)) {
        RemoveIndicator();
        GameManager.Instance.player.NormalMovement.DisableMoving();
        manager.StartDialog();
      }
    }

    #endregion

    #region Dialog API
    //---------------------------------------------------------------------
    // Dialog API
    //---------------------------------------------------------------------

    /// <summary>
    /// Begin a conversation.
    /// </summary>
    public void StartDialog() {
      manager.StartDialog();
    }

    /// <summary>
    /// Continue an existing conversation.
    /// </summary>
    public void NextSentence() {
      manager.NextSentence();
    }

    /// <summary>
    /// End a conversation.
    /// </summary>
    public void EndDialog() {
      manager.EndDialog();
    }

    /// <summary>
    /// Set the current dialog
    /// </summary>
    /// <param name="dialog"></param>
    public void SetCurrentDialog(DialogGraph dialog) {
      manager.SetCurrentDialog(dialog);
    }

    /// <summary>
    /// Add the dialog indicator above the player.
    /// </summary>
    public void AddIndicator() {
      PlayerCharacter player = GameManager.Instance.player;
      indicatorInstance = Instantiate<GameObject>(
        indicatorPrefab,
        player.transform.position + indicatorPosition,
        Quaternion.identity
      );

      indicatorInstance.transform.parent = player.transform;
      manager.canStartConversation = true;
    }

    /// <summary>
    /// Remove the dialog indicator from the player.
    /// </summary>
    public void RemoveIndicator() {
      if (indicatorInstance != null) {
        Destroy(indicatorInstance.gameObject);
      }
      manager.canStartConversation = false;
    }

    #endregion
  }
}