#if UNITY_EDITOR
using UnityEditor.Animations;
using UnityEditor;
#endif

using UnityEngine;

namespace Storm.Extensions {
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
  }
}