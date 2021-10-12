#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
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

    public static IEnumerator EnumerateTween(Vector3 start, Vector3 end, int steps) {
      foreach (Vector3 pos in MakeTween(start, end, steps)) {
        yield return pos;
      }
    }

    public static List<Vector3> MakeTween(Vector3 start, Vector3 end, int steps) {
      float t = 0;
      float stepLength = 1/steps;

      List<Vector3> positions = new List<Vector3>();
      while (t < 1) {
        t = Mathf.Clamp(t+stepLength, 0, 1);
        positions.Add(Vector3.Lerp(start, end, t));
      }

      return positions;
    }
  }
}