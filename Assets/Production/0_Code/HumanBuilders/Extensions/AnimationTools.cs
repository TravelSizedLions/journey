#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
#endif

using UnityEngine;

namespace HumanBuilders {
  public static class AnimationTools {

#if UNITY_EDITOR
    /// <summary>
    /// Get the animator controller asset bound to a given animator.
    /// </summary>
    /// <param name="animator">The animator.</param>
    /// <returns>The animator controller asset bound to the animator.</returns>
    public static AnimatorController GetController(Animator animator) {
      RuntimeAnimatorController runtimeController = animator.runtimeAnimatorController;
      if (runtimeController == null) {
        return null;
      }

      AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(AssetDatabase.GetAssetPath(runtimeController));
      return controller;
    }
#endif

    public static bool HasParameter(Animator animator, string paramName) {
      if (animator == null) {
        return false;
      }
      
      foreach (AnimatorControllerParameter param in animator.parameters) {
        if (param.name == paramName)
          return true;
      }
      return false;
    }
  }
}