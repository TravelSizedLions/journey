using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.SHORT)]
  [CreateNodeMenu("Camera/Shake")]
  public class ShakeCameraNode : AutoNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [PropertyOrder(0)]
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(5)]
    [PropertyOrder(999)]
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    [Space(3)]
    [BoxGroup("Settings")]
    [Tooltip("How much the camera should shake.")]
    public float Intensity = .1f;

    [BoxGroup("Settings")]
    [Tooltip("How long the camera should shake (in seconds).")]
    public float Duration = .25f;

    [BoxGroup("Settings")]
    [Tooltip("How long the camera should wait before shaking (in seconds).")]
    public float DelayBefore = 0f;

    [Space(5)]
    [BoxGroup("Settings")]
    [Tooltip("Wait until the camera stops shaking to continue.")]
    public bool PauseGraph;

    [BoxGroup("Settings")]
    [ShowIf("PauseGraph")]
    public float DelayAfter = 0f;
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (PauseGraph) {
        if (graphEngine.LockNode()) {
          new UnityTask(Shake(graphEngine));
        }
      } else {
        GameManager.CurrentTargettingCamera.CameraShake(Duration, DelayBefore, Intensity);
      }
    }

    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    private IEnumerator Shake(GraphEngine graphEngine) {
      GameManager.CurrentTargettingCamera.CameraShake(Duration, DelayBefore, Intensity);
      
      while(GameManager.CurrentTargettingCamera.Shaking) {
        yield return null;
      }

      yield return new WaitForSeconds(DelayAfter);

      if (PauseGraph) {
        graphEngine.UnlockNode();
      }
    }
  }
}