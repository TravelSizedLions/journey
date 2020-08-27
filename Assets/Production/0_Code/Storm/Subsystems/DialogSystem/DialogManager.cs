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

    #region Fields
    //---------------------------------------------------
    // Fields
    //---------------------------------------------------
      
    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private IPlayer player;

    #region Dialog Boxes
    //---------------------------------------------------
    // Dialog Boxes
    //---------------------------------------------------

    /// <summary>
    /// A map of Dialog Boxes that can be opened/closed, by name.
    /// </summary>
    private Dictionary<string, DialogBox> dialogBoxes;


    /// <summary>
    /// The dialog box that's currently open.
    /// </summary>
    private DialogBox openDialogBox;


    [Space(10, order=0)]

    /// <summary>
    /// The dialog box that will be used by default for any
    /// </summary>
    [Tooltip("The dialog box that will be opened by default at the start of every conversation and inspection.")]
    public DialogBox DefaultDialogBox;
    #endregion

    #region Dialog Graph Model
    //---------------------------------------------------
    // Dialog Graph Model
    //---------------------------------------------------

    /// <summary>
    /// The current conversation being played out.
    /// </summary>
    private IDialog currentDialog;

    /// <summary>
    /// The current dialog node.
    /// </summary>
    private IDialogNode currentNode;
    #endregion

    #region Management Flags
    //---------------------------------------------------
    // Management Flags
    //---------------------------------------------------

    [Header("Conversation State Management", order = 6)]
    [Space(5, order = 7)]

    /// <summary>
    /// Whether or not the manager is currently busy managing a node in the conversation.
    /// </summary>
    [Tooltip("Whether or not the manager is currently busy managing the conversation.")]
    [SerializeField]
    [ReadOnly]
    public bool handlingNode;


    /// <summary>
    /// Whether or not the current node in the dialog has locked progress in the converation.
    /// </summary>
    [Tooltip("Whether or not the current node in the dialog has locked progress in the converation.")]
    [SerializeField]
    [ReadOnly]
    private bool nodeLocked;
    
    /// <summary>
    /// Whether or not the text is still being written to the screen.
    /// </summary>
    [Tooltip("Whether or not the text is still being written to the screen.")]
    [ReadOnly]
    public bool StillWriting;
    #endregion
    #endregion

    #region Unity API
    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------
      
    protected void Start() {
      player = FindObjectOfType<PlayerCharacter>();

      SceneManager.sceneLoaded += OnNewScene;

      DialogBox[] boxes = GetComponentsInChildren<DialogBox>();
      if (DefaultDialogBox == null && boxes.Length == 1) {
        DefaultDialogBox = boxes[0];
      }

      dialogBoxes = new Dictionary<string, DialogBox>();
      foreach (DialogBox box in boxes) {
        if (!dialogBoxes.ContainsKey(box.name)) {
          dialogBoxes.Add(box.name, box);
        } else {
          Debug.LogWarning("A Dialog Box named \"" + box.name + "\" has already been added to the DialogManager");
        }
      }
    }
    #endregion

    #region Dependency Injection
    //---------------------------------------------------------------------
    // Dependency Injection
    //---------------------------------------------------------------------
      
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
     

    #region Top-Level Interface
    //---------------------------------------------------------------------
    // Top Level Interaction
    //---------------------------------------------------------------------
      
    /// <summary>
    /// Begins a new dialog with the player.
    /// </summary>
    public void StartDialog(IDialog graph) {
      if (graph == null) {
        throw new UnityException("No dialog has been set!");
      }
      
      Debug.Log("Start Dialog!");

      if (player == null) {
        player = GameManager.Instance.player;
      }

      player.DisableJump();
      player.DisableMove();
      player.DisableCrouch();

      SetCurrentDialog(graph);

      if (!handlingNode) {
        handlingNode = true;

        currentNode = currentDialog.StartDialog();

        if (currentNode == null) {
          return;
        }

        openDialogBox = DefaultDialogBox;
        openDialogBox.Open();

        handlingNode = false;
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
      if (player == null) {
        player = GameManager.Instance.player;
      }
      
      player.EnableJump();
      player.EnableCrouch();
      player.EnableMove();


      if (openDialogBox != null) {
        openDialogBox.Close();
        openDialogBox = null;
      }
    }
    #endregion

    #region Dialog UI Manipulation
    //---------------------------------------------------------------------
    // Dialog UI Manipulation
    //---------------------------------------------------------------------
      
    /// <summary>
    /// Type out a sentence.
    /// </summary>
    /// <param name="sentence">The sentence to type.</param>
    /// <param name="speaker">The speaker saying it, if any.</param>
    public void Type(string sentence, string speaker = "") {
      if (openDialogBox != null) {
        openDialogBox.Type(sentence, speaker);
      } else {
        Debug.LogWarning("There's no dialog box currently open!");
      }
    }

    /// <summary>
    /// Remove the decision buttons from the screen.
    /// </summary>
    public void ClearDecisions() {
      openDialogBox.ClearDecisions();
    }


    /// <summary>
    /// Open the dialog box with a given name. If no name is provided, the
    /// default dialog box will be opened.
    /// </summary>
    /// <param name="name">The name of the dialog box to open.</param>
    public void OpenDialogBox(string name = "") {
      if (string.IsNullOrEmpty(name)) {
        OpenDefaultDialogBox();
      } else {

        if (dialogBoxes.ContainsKey(name)) {
          if (openDialogBox == null) {
            openDialogBox = dialogBoxes[name];
            openDialogBox.Open();
          } else {
            SwitchToDialogBox(name);
          }
        } else {
          Debug.LogWarning("The Dialog Box \"" + name + "\" doesn't exist!");
        }

      }
    }

    /// <summary>
    /// Opens or switches to the default dialog box.
    /// </summary>
    private void OpenDefaultDialogBox() {
      if (openDialogBox != null) {
        openDialogBox.Close();
      }

      openDialogBox = DefaultDialogBox;
      openDialogBox.Open();
    }

    /// <summary>
    /// Switch to a dialog box of a given name. If no name is provided, the
    /// default dialog box will be opened.
    /// </summary>
    /// <param name="name">The name of the dialog box to switch to.</param>
    public void SwitchToDialogBox(string name = "") {
      if (string.IsNullOrEmpty(name)) {
        OpenDefaultDialogBox();
      } else {

        if (dialogBoxes.ContainsKey(name)) {
          if (openDialogBox != null) {
            openDialogBox.Close();
            openDialogBox = dialogBoxes[name];
            openDialogBox.Open();
          } else {
            OpenDialogBox(name);
          }
        } else {
          Debug.LogWarning("The Dialog Box \"" + name + "\" doesn't exist!");
        }

      }
    }

    /// <summary>
    /// Close the current dialog box.
    /// </summary>
    public void CloseDialogBox() {
      if (openDialogBox != null) {
        openDialogBox.Close();
        openDialogBox = null;
      } else {
        Debug.LogWarning("There is no dialog box open currently!");
      }
    }

    #endregion 

    #region Getters/Setters
    //---------------------------------------------------------------------
    // Getters/Setters
    //---------------------------------------------------------------------

    /// <summary>
    /// Set the current node in the dialog graph.
    /// </summary>
    public void SetCurrentNode(IDialogNode node) {
      currentNode = node;
    }

    /// <summary>
    /// Get the current node in the dialog graph.
    /// </summary>
    public IDialogNode GetCurrentNode() {
      return currentNode;
    }

    /// <summary>
    /// Set the current dialog that should be handled.
    /// </summary>
    public void SetCurrentDialog(IDialog dialog) {
      currentDialog = dialog;
    }

    /// <summary>
    /// Whether or not the dialog has completed.
    /// </summary>
    public bool IsDialogFinished() {
      // End nodes should set the current node to null themselves.
      return currentNode == null;
    }

    /// <summary>
    /// Get the on screen decision buttons.
    /// </summary>
    /// <returns>The list of decision buttons on screen.</returns>
    public List<GameObject> GetDecisionButtons() {
      return openDialogBox.GetDecisionButtons();
    }

    #endregion
      
    /// <summary>
    /// How the dialog manager should handle the loading of a new scene.
    /// </summary>
    private void OnNewScene(Scene aScene, LoadSceneMode aMode) {
      player = GameManager.Instance.player;
    }

    /// <summary>
    /// Locks handling a dialog. This will prevent more nodes from being fired
    /// in a conversation until the lock has been released.
    /// </summary>
    /// <returns>True if the lock was obtained, false otherwise.</returns>
    public bool LockNode() {
      if (nodeLocked) {
        return false;
      }

      nodeLocked = true;
      return true;
    }

    /// <summary>
    /// Unlocks handling a dialog. If there was previously a lock on firing more
    /// nodes in the conversation, this will release it.
    /// </summary>
    /// <remarks>
    /// Don't use this without first trying to obtain the lock for yourself.
    /// </remarks>
    public void UnlockNode() {
      nodeLocked = false;
    }

    /// <summary>
    /// Try to start handling a node in the conversation.
    /// </summary>
    /// <returns>
    /// True if previous node in the conversation graph is finished being handled. False otherwise.
    /// </returns>
    public bool StartHandlingNode() {
      if (!nodeLocked) {
        handlingNode = true;
        return true;
      } else {
        return false;
      }
    }


    /// <summary>
    /// Try to finish handling a node in the conversation.
    /// </summary>
    /// <returns>
    /// True if the current node finished handling successfully. False if the current node still needs time to finish.
    /// </returns>
    public bool FinishHandlingNode() {
      if (!nodeLocked) {
        handlingNode = false;
        return true;
      } else {
        return false;
      }
    }


  }
}
