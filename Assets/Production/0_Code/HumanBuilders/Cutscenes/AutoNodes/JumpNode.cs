using System.Collections;
using Sirenix.OdinInspector;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// This node has the player automatically walk towards a target position.
  /// </summary>
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.NORMAL)]
  [CreateNodeMenu("Player/Jump")]
  public class JumpNode : AutoNode {

    //-------------------------------------------------------------------------
    // Node Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8)]

    /// <summary>
    /// How long to wait after triggering a jump.
    /// </summary>
    [Tooltip("How long to wait after triggering a jump.")]
    public float DelayAfter = 0.5f;

    [Space(8)]

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {


      if (graphEngine.LockNode()) {
        new UnityTask(Jump(graphEngine));
      }
    }

    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    private IEnumerator Jump(GraphEngine graphEngine) {
      GameManager.Player.EnableJump(DialogManager.Instance);

      IInputComponent playerInput = GameManager.Player.PlayerInput;
      IInputComponent vInput = new VirtualInput();
      GameManager.Player.PlayerInput = vInput;
      GameManager.GamePad.PressButton("Jump");

      yield return null;

      GameManager.Player.PlayerInput = playerInput;

      GameManager.Player.DisableJump(DialogManager.Instance);

      if (DelayAfter > 0) {
        yield return new WaitForSeconds(DelayAfter);
      }
      graphEngine.UnlockNode();
    }
  }
}