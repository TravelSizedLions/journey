using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  [NodeWidth(300)]
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [CreateNodeMenu("Animation/Tween Position")]
  public class TweenNode : AutoNode {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    /// <summary>
    /// The output connection for this node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    /// <summary>
    /// The Game Object to move.
    /// </summary>
    [Tooltip("The Game Object to move.")]
    public Transform Target;

    /// <summary>
    /// The position to move the Game Object to.
    /// </summary>
    [Tooltip("The position to move the Game Object to.")]
    public Transform Destination;

    /// <summary>
    /// Whether or not to stop graph execution while tweening.
    /// </summary>
    [Tooltip("Whether or not to stop graph execution while tweening.")]
    public bool PauseGraph = true;

    /// <summary>
    /// The amount of time it should take to move from Target position to Destination.
    /// </summary>
    [Tooltip("The amount of time it should take to move from Target position to Destination.")]
    [LabelText("Duration (s)")]
    public float Duration;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (Target == null) {
        Debug.Log("Tween Node in graph \"" + graphEngine.GetCurrentGraph().GraphName + "\" is missing a target to move.");
        return;
      }

      if (Destination == null) {
        Debug.Log("Tween Node in graph \"" + graphEngine.GetCurrentGraph().GraphName + "\" is missing a destination.");
        return;
      }

      if (PauseGraph) {
        if (graphEngine.LockNode()) {
          new UnityTask(Tween(graphEngine));
        }
      } else {
        new UnityTask(Tween(graphEngine));
      }
    }

    private IEnumerator Tween(GraphEngine graphEngine) {
      Vector3 start = Target.position;
      Vector3 end = Destination.position;

      float elapsedTime = 0;
      while (elapsedTime < Duration) {
        elapsedTime = Mathf.Clamp(elapsedTime+Time.deltaTime, 0, Duration);
        float normalized = Mathf.Clamp(elapsedTime/Duration, 0, 1);
        Vector3 interpPosition = start*(1-normalized) + end*(normalized);
        Target.position = interpPosition;
  
        yield return null;
      }

      if (PauseGraph) {
        graphEngine.UnlockNode();
      }
    }
  }
}