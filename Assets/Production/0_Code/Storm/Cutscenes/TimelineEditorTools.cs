#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEditor;

namespace HumanBuilders {
  /// <summary>
  /// A collection of functions to help with timeline related tasks.
  /// </summary>
  public static class TimelineEditorTools {


    /// <summary>
    /// Create empty curves for any properties missing on the given timeline clip.
    /// </summary>
    /// <param name="clip">The timeline clip to populate.</param>
    /// <param name="type">The type used for the curve (used by
    /// AnimationCurve.SetCurve). This should most likely be the concrete class
    /// for the PlayableAsset clip hosting these curves.</param>
    /// <param name="propertyNames">The names of the properties desired on the clip.</param>
    /// <param name="defaultValues">The list of default curve values desired,
    /// coinciding with the list of property names.</param>
    public static void CreateEmptyCurves(TimelineClip clip, Type type, IList<string> propertyNames, IList<float> defaultValues = null) {
      if (clip.curves != null) {
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip.curves);

        IList<string> missing = FindMissingProperties(bindings, propertyNames);
        CreateMissingProperties(clip, type, missing, propertyNames, defaultValues);

      } else {
        Debug.Log("missing curve?");
      }

      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();
    }

    /// <summary>
    /// Given a list of property names, find which properties are missing from
    /// the list of curves.
    /// </summary>
    /// <param name="bindings">The curves bound to a given clip.</param>
    /// <param name="propertyNames">The property names to search for.</param>
    /// <returns>A list of the properties that are missing</returns>
    private static IList<string> FindMissingProperties(EditorCurveBinding[] bindings, IList<string> propertyNames) {
      foreach (EditorCurveBinding binding in bindings) {
        propertyNames.Remove(binding.propertyName);
      }

      return propertyNames;
    }

    /// <summary>
    /// Given a list of properties, create an empty curve for each property with
    /// a default keyframe at time 0.
    /// </summary>
    /// <param name="clip">The timeline clip to create curves on.</param>
    /// <param name="type">The type used for the curve (used by AnimationCurve.SetCurve)</param>
    /// <param name="missing">A list of the missing properties.</param>
    /// <param name="propertyNames">The full list of desired properties for the clip</param>
    /// <param name="defaultValues">The list of default keyframe values for the desired properties</param>
    private static void CreateMissingProperties(TimelineClip clip, Type type, IList<string> missing, IList<string> propertyNames, IList<float> defaultValues = null) {
      bool useDefaults = defaultValues != null;

      // Create a curve with a keyframe at time 0 for each missing property.
      foreach(string prop in missing) {
        if (useDefaults) {
          TimelineEditorTools.CreateEmptyCurve(clip, type, prop, defaultValues[propertyNames.IndexOf(prop)]);
        } else {
          TimelineEditorTools.CreateEmptyCurve(clip, type, prop);
        }
      }
    }

    /// <summary>
    /// Create a default curve on the given clip.
    /// </summary>
    /// <param name="clip">The timeline clip to populate.</param>
    /// <param name="type">The type used for the curve (used by
    /// AnimationCurve.SetCurve). This should most likely be the concrete class
    /// for the PlayableAsset clip hosting these curves.</param>
    /// <param name="propertyName">The name of the property the curve is for</param>
    /// <param name="defaultValue">The starting value for the curve</param>
    public static EditorCurveBinding CreateEmptyCurve(TimelineClip clip, Type type, string propertyName, float defaultValue = 0) {
      AnimationCurve curve = new AnimationCurve(new Keyframe[] { new Keyframe(0, defaultValue)});
      clip.curves.SetCurve("", type, propertyName, curve);

      // Create the equivalent binding
      EditorCurveBinding binding = new EditorCurveBinding();
      binding.type = type;
      binding.propertyName = propertyName;
      binding.path = "";
      return binding;
    }

  }
}
#endif