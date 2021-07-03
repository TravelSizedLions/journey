using System.Collections;
using System.Collections.Generic;

using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders {

  /// <summary>
  /// A manager for conversations with NPCs and the like. This is essentially a Decorator/Wrapper around the
  /// Graph traversal engine.
  /// </summary>
  /// <remarks>
  /// Decorator/Wrapper Pattern: 
  /// https://www.tutorialspoint.com/design_pattern/decorator_pattern.htm
  /// https://refactoring.guru/design-patterns/decorator
  /// </remarks>
  /// <seealso cref="AutoNode" />
  /// <seealso cref="AutoGraphAsset" />
  /// <seealso cref="GraphEngine" />
  public class DialogManager : Singleton<DialogManager> {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The Dialog Manager's graph engine. This is responsible for traversing
    /// the graph.
    /// </summary>
    /// <seealso cref="AutoNode" />
    /// <seealso cref="AutoGraph" />
    public static GraphEngine GraphEngine { get { return Instance.graphEngine; } }

    public static Dictionary<char, float> Punctuation = new Dictionary<char, float>() {
        {'.', 0.25f},
        {'?', 0.25f},
        {'!', 0.25f},
    };

    /// <summary>
    /// The UI canvas for dialog.
    /// </summary>
    public static Canvas Canvas { 
      get { 
        if (dialogCanvas == null) {
          dialogCanvas = Instance.GetComponent<Canvas>();
        }
        return dialogCanvas;
      }
    }

    //---------------------------------------------------
    // Fields
    //---------------------------------------------------
    // /// <summary>
    // /// The default character profile to reset dialog boxes to when the dialog
    // /// has finished.
    // /// </summary>
    // [Tooltip("The default character profile to reset dialog boxes to when the dialog has finished.")]
    // public CharacterProfile DefaultCharacterProfile;

    /// <summary>
    /// A reference to the player character.
    /// </summary>
    private IPlayer player;

    /// <summary>
    /// A map of Dialog Boxes that can be opened/closed, by name.
    /// </summary>
    private Dictionary<string, IDialogBox> dialogBoxes;

    /// <summary>
    /// The dialog box that's currently open.
    /// </summary>
    private IDialogBox openDialogBox;

    [Space(10, order=0)]

    /// <summary>
    /// The dialog box that will be opened by default at the start of every conversation/inspection.
    /// </summary>
    [Tooltip("The dialog box that will be opened by default at the start of every conversation.")]
    public IDialogBox DefaultDialogBox;

    /// <summary>
    /// The graph traversal engine. The dialog manager delegates running a branching
    /// conversation off to this.
    /// </summary>
    private GraphEngine graphEngine;
    
    /// <summary>
    /// Whether or not text is currently being written to the screen.
    /// </summary>
    [Tooltip("Whether or not text is currently being written to the screen.")]
    [ReadOnly]
    public bool StillWriting;

    /// <summary>
    /// The UI canvas for dialog.
    /// </summary>
    private static Canvas dialogCanvas;

    //---------------------------------------------------------------------
    // Unity API
    //---------------------------------------------------------------------
    protected void Start() {
      graphEngine = gameObject.AddComponent<GraphEngine>();
      graphEngine.OnGraphEnded += OnDialogEnded;

      player = FindObjectOfType<PlayerCharacter>();
      SceneManager.sceneLoaded += OnNewScene;

      IDialogBox[] boxes = GetComponentsInChildren<DialogBox>();
      if (DefaultDialogBox == null && boxes.Length == 1) {
        DefaultDialogBox = boxes[0];
      }

      dialogBoxes = new Dictionary<string, IDialogBox>();
      foreach (IDialogBox box in boxes) {
        if (!dialogBoxes.ContainsKey(box.name)) {
          dialogBoxes.Add(box.name, box);
        } else {
          Debug.LogWarning("A Dialog Box named \"" + box.name + "\" has already been added to the DialogManager");
        }
      }

      dialogCanvas = GetComponent<Canvas>();
    }

    //---------------------------------------------------------------------
    // Dependency Injection
    //---------------------------------------------------------------------
    /// <summary>
    /// Unit test dependency injection point for the graphing engine.
    /// </summary>
    /// <param name="graphEngine">The graphing engine.</param>
    public static void Inject(GraphEngine graphEngine) {
      Instance.graphEngine = graphEngine;
      Instance.graphEngine.OnGraphEnded += Instance.OnDialogEnded;
    }

    /// <summary>
    /// Dependency injection point for a reference to the player.
    /// </summary>
    /// <param name="player">A reference to the player.</param>
    public static void Inject(IPlayer player) {
      Instance.player = player;
    }

    /// <summary>
    /// Dependency injection point for a Dialog graph.
    /// </summary>
    /// <param name="dialog">The dialog to inject</param>
    public static void Inject(IAutoGraph dialog) {
      Instance.graphEngine.Inject(dialog);
    }

    /// <summary>
    /// Dependency injection point for a dialog node.
    /// </summary>
    /// <param name="node">The node to inject.</param>
    public static void Inject(IAutoNode node) {
      Instance.graphEngine.Inject(node);
    }


    /// <summary>
    /// Dependency injection point for a dialog box.
    /// </summary>
    /// <param name="dialogBox">The dialog box to inject.</param>
    /// <param name="open">Whether or not the dialog box should be opened immediately.</param>
    public static void Inject(IDialogBox dialogBox, bool open) {
      Instance.DefaultDialogBox = dialogBox;
      if (open) {
        Instance.openDialogBox = dialogBox;
        Instance.openDialogBox.Open();
      }
    }     

    //---------------------------------------------------------------------
    // Top Level Interaction
    //---------------------------------------------------------------------
    /// <summary>
    /// Begins a new dialog with the player.
    /// </summary>
    public static void StartDialog(IAutoGraph graph) => Instance.StartDialog_Inner(graph);
    private void StartDialog_Inner(IAutoGraph graph) {
      if (graph == null) {
        throw new UnityException("No dialog has been set!");
      }

      if (player == null) {
        player = GameManager.Player;
      }

      player.DisableJump(this);
      player.DisableMove(this);
      player.DisableCrouch(this);

      openDialogBox = DefaultDialogBox;
      openDialogBox.Open();

      graphEngine.StartGraph(graph);
    }

    /// <summary>
    /// Continues the dialog.
    /// </summary>
    public static void ContinueDialog() => Instance.graphEngine?.Continue();

    
    /// <summary>
    /// End the current dialog.
    /// </summary>
    public static void EndDialog() => Instance.graphEngine?.EndGraph();

    /// <summary>
    /// Delegate for the graph engine for when it reaches the end of the graph.
    /// </summary>
    private void OnDialogEnded() {
      if (player == null) {
        player = GameManager.Player;
      }
      
      player.EnableJump(this);
      player.EnableCrouch(this);
      player.EnableMove(this);

      if (openDialogBox != null) {
        // if (DefaultCharacterProfile != null) {
        //   openDialogBox.ApplyColors(
        //     DefaultCharacterProfile.PrimaryColor,
        //     DefaultCharacterProfile.SecondaryColor,
        //     DefaultCharacterProfile.TextColor
        //   );
        // }
        openDialogBox.Close();
        openDialogBox = null;
      }
      
      if (player.CurrentInteractible != null) {
        player.CurrentInteractible.EndInteraction();
      }
    }

    //---------------------------------------------------------------------
    // Dialog UI Manipulation
    //---------------------------------------------------------------------
    /// <summary>
    /// Type out a sentence.
    /// </summary>
    /// <param name="sentence">The sentence to type.</param>
    /// <param name="speaker">The speaker saying it, if any.</param>
    /// <param name="autoAdvance">Whether or not to automatically advance the
    /// dialog after typing has finished.</param>
    /// <param name="delayBeforeAdvance">How long to delay before advancing the
    /// dialog, in seconds</param>
    public static void Type(string sentence, string speaker = "", bool autoAdvance = false, float delayBeforeAdvance = 0f) => Instance.Type_Inner(sentence, speaker, autoAdvance, delayBeforeAdvance);
    private void Type_Inner(string sentence, string speaker = "", bool autoAdvance = false, float delayBeforeAdvance = 0f) {
      if (openDialogBox != null) {
        openDialogBox.Type(sentence, speaker, autoAdvance, delayBeforeAdvance);
      } else {
        Debug.LogWarning("There's no dialog box currently open!");
      }
    }

    /// <summary>
    /// Clear out the text and speaker from the open dialog box.
    /// </summary>
    public static void ClearText() => Instance.openDialogBox.ClearText();

    /// <summary>
    /// Remove the decision buttons from the screen.
    /// </summary>
    public static void ClearDecisions() => Instance.openDialogBox.ClearDecisions();


    /// <summary>
    /// Open the dialog box with a given name. If no name is provided, the
    /// default dialog box will be opened.
    /// </summary>
    /// <param name="name">The name of the dialog box to open.</param>
    public static void OpenDialogBox(string name = "") => Instance.OpenDialogBox_Inner(name);
    private void OpenDialogBox_Inner(string name = "") {
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
    public static void SwitchToDialogBox(string name = "") => Instance.SwitchToDialogBox_Inner(name);
    private void SwitchToDialogBox_Inner(string name = "") {
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
    public static void CloseDialogBox() => Instance.CloseDialogBox_Inner();
    private void CloseDialogBox_Inner() {
      if (openDialogBox != null) {
        openDialogBox.Close();
        openDialogBox = null;
      } else {
        Debug.LogWarning("There is no dialog box open currently!");
      }
    }

    /// <summary>
    /// Whether or not there's an open dialog box.
    /// </summary>
    public static bool IsDialogBoxOpen() => Instance.openDialogBox != null;

    /// <summary>
    /// Use a certain character profile when dealing with dialog.
    /// </summary>
    /// <param name="profile">The profile to use.</param>
    public static void UseCharacterProfile(CharacterProfile profile) => Instance.openDialogBox.ApplyColors(
      profile.PrimaryColor,
      profile.SecondaryColor,
      profile.TextColor
    );

    /// <summary>
    /// Use the default set of colors that comes with the open dialog box.
    /// </summary>
    public static void UseDefaultDialogColors() => Instance.openDialogBox.ResetColors();

    //---------------------------------------------------------------------
    // Getters/Setters
    //---------------------------------------------------------------------
    /// <summary>
    /// Set the current node in the dialog graph.
    /// </summary>
    public static void SetCurrentNode(IAutoNode node) => Instance.graphEngine.SetCurrentNode(node);


    /// <summary>
    /// Get the current node in the dialog graph. Don't use this while in the
    /// middle of another dialog.
    /// </summary>
    public static IAutoNode GetCurrentNode() => Instance.graphEngine.GetCurrentNode();

    /// <summary>
    /// Set the current dialog that should be handled. Don't use this while in
    /// the middle of another dialog.
    /// </summary>
    public static void SetCurrentDialog(IAutoGraph dialog) => Instance.graphEngine.SetCurrentGraph(dialog);

    /// <summary>
    /// Whether or not the dialog has completed.
    /// </summary>
    public static bool IsDialogFinished() => Instance.graphEngine.IsFinished();

    /// <summary>
    /// Get the on screen decision buttons.
    /// </summary>
    /// <returns>The list of decision buttons on screen.</returns>
    public static List<DecisionBox> GetDecisionButtons() => Instance.GetDecisionButtons_Inner();
    private List<DecisionBox> GetDecisionButtons_Inner() {
      return openDialogBox.GetDecisionButtons();
    }
      
    /// <summary>
    /// How the dialog manager should handle the loading of a new scene.
    /// </summary>
    private void OnNewScene(Scene aScene, LoadSceneMode aMode) {
      player = GameManager.Player;
    }
  }
}
