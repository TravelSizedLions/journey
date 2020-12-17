using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Characters.Player;
using Storm.Inputs;
using Storm.Subsystems.Dialog;
using Storm.Subsystems.Graph;
using UnityEngine;

namespace Storm.Cutscenes {

  /// <summary>
  /// This node has the player automatically walk towards a target position.
  /// </summary>
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(400)]
  [CreateNodeMenu("Animation/Walk to Position")]
  public class WalkToPositionNode : AutoNode {

    #region Node Fields
    //-------------------------------------------------------------------------
    // Node Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The place to move the player to.
    /// </summary>
    [Tooltip("The place to move the player to.")]
    public Transform target = null;

    /// <summary>
    /// How fast the player should move towards their target. 0 - No movement, 
    /// 1 - Move at the max player speed.
    /// </summary>
    [Tooltip("How fast the player should move towards their target. 0 - No movement, 1 - Move at the max player speed.")]
    [Range(0, 1)]
    public float Speed = 1f;

    [Space(5, order=1)]

    /// <summary>
    /// Don't advance the graph until the player has arrived at the target position.
    /// </summary>
    [Tooltip("Don't advance the graph until the player has arrived at the target position.")]
    public bool PauseGraph = true;

    /// <summary>
    /// How much longer to wait after the player has arrived at the target before advancing the graph.
    /// </summary>
    [Tooltip("How much longer to wait after the player has arrived at the target before advancing the graph.")]
    [ShowIf("PauseGraph")]
    public float DelayAfter = 0.5f;

    [Space(8, order=2)]

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection Output;


    #endregion

    #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (target != null) {
        graphEngine.StartThread(WalkToPosition.MoveTo(target, Speed, graphEngine, PauseGraph, DelayAfter));
      } else {
        Debug.LogWarning("WalkToPosition Node in graph \"" +  graphEngine.GetCurrentGraph().Name + "\" is missing a target position for the player to walk to. Go into the AutoGraph editor for this graph and find the node with the missing target.");
      }
    }
    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Walk to in one direction until reaching the target.
    /// </summary>
    private IEnumerator Walk(GraphEngine graphEngine) {
      bool canContinue = !PauseGraph || graphEngine.LockNode();

      if (canContinue) {

        player.EnableMove(DialogManager.Instance);
        bool walkLeft = GameManager.Player.Physics.Px > target.position.x;

        // Temporarily store the real player input.
        IInputComponent playerInput = GameManager.Player.PlayerInput;
        GameManager.Player.PlayerInput = new VirtualInput();

        StartWalking(walkLeft);

        while (!ShouldStop(walkLeft)) {
          yield return null;
        }

        StopWalking(playerInput);

        if (PauseGraph) {
          yield return new WaitForSeconds(DelayAfter);
          graphEngine.UnlockNode();
        }

        player.DisableMove(DialogManager.Instance);
      }
    }

    /// <summary>
    /// Have the player begin walking either left or right.
    /// </summary>
    /// <param name="walkLeft">Whether or not the player should walk left or right.</param>
    private void StartWalking(bool walkLeft) {
      float input = walkLeft ? -Speed : Speed;
      GameManager.GamePad.SetHorizontalAxis(input);
    }

    /// <summary>
    /// Have the player stop walking.
    /// </summary>
    /// <param name="playerInput">The player's original input component</param>
    private void StopWalking(IInputComponent playerInput) {
      GameManager.GamePad.SetHorizontalAxis(0);
      GameManager.Player.PlayerInput = playerInput;
    }

    /// <summary>
    /// Whether or not the player should stop moving towards the target.
    /// </summary>
    private bool ShouldStop(bool walkLeft) {
      if (walkLeft) {
        return GameManager.Player.Physics.Px <= target.position.x; 
      } else {
        return GameManager.Player.Physics.Px >= target.position.x;
      }
    }

    #endregion
  }

}