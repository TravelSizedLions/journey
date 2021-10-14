﻿using System.Collections;
using Sirenix.OdinInspector;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// This node has the player automatically walk towards a target position.
  /// </summary>
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.NORMAL)]
  [CreateNodeMenu("Player/Walk to Position")]
  public class WalkToPositionNode : AutoNode {

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

    /// <summary>
    /// Which way the player should face after arriving at the destination.
    /// </summary>
    [Tooltip("Which way the player should face after arriving at the destination.")]
    public Facing FacingAfter = Facing.None;

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

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (target != null) {
        if (PauseGraph) {
          if (graphEngine.LockNode()) {
            new UnityTask(TryWalk(graphEngine));   
          }  
        } else {
          new UnityTask(TryWalk(graphEngine));
        }

      } else {
        Debug.LogWarning("WalkToPosition Node in graph \"" +  graphEngine.GetCurrentGraph().GraphName + "\" is missing a target position for the player to walk to. Go into the AutoGraph editor for this graph and find the node with the missing target.");
      }
    }

    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    private IEnumerator TryWalk(GraphEngine graphEngine) {
      UnityTask walking = WalkToPosition.WalkTo(target, Speed, graphEngine, PauseGraph, DelayAfter);
      
      while(walking.Running) {
        yield return null;
      }

      if (FacingAfter != Facing.None) {
        GameManager.Player.SetFacing(FacingAfter);
      }

      yield return new WaitForSeconds(DelayAfter);

      if (PauseGraph) {
        graphEngine.UnlockNode();
      }
    }
  }

}