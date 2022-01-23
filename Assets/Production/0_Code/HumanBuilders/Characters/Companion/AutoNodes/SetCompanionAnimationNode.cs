using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

namespace HumanBuilders {

  /// <summary>
  /// This node has the player automatically walk towards a target position.
  /// </summary>
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.SHORT)]
  [CreateNodeMenu("Companion/Set Animation")]
  public class SetCompanionAnimationNode : AnimationParameterNode {
    private const string COMPANION_ANIM_PATH = "";

    private const string UNSELECTED = "-- select a parameter --";

    public override void Handle(GraphEngine graphEngine) {
      if (GameManager.Companion == null) {
        Debug.LogWarning("Player companion is currently disabled in-game.");
        return;
      }

      Animator = GameManager.Companion.GetComponentInChildren<Animator>();
      if (Animator == null) {
        Debug.LogWarning(string.Format("Could not find animator in companion \"{0}\"", GameManager.Companion.name));
        return;
      }
  
      base.Handle(graphEngine);
    }

    //-------------------------------------------------------------------------
    // Odin Inspector Stuff
    //-------------------------------------------------------------------------
    protected override IEnumerable Params() {
      List<string> parms = new List<string>();

      #if UNITY_EDITOR
      AnimatorController anim = DeveloperSettings.GetSettings().CompanionAnimatorController;

      if (anim != null) {
        if (anim.parameters.Length > 0) {
          foreach (var param in anim.parameters) {
            parms.Add(param.name);
          }
        }
      }

      if (Parameter == null) {
        Parameter = UNSELECTED;
      }
      #endif

      return parms;
    }

    protected override void SetParamType() {
      if (Parameter != null && Parameter != UNSELECTED) {
        #if UNITY_EDITOR
        AnimatorController anim = DeveloperSettings.GetSettings().CompanionAnimatorController;

        if (anim != null) {
          foreach (var param in anim.parameters) {
            if (param.name == Parameter) {
              parameterType = param.type;
            }
          }
        }
        #endif
      }
    }
  }
}