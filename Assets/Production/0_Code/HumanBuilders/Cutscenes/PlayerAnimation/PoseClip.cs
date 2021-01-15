using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
#endif

namespace HumanBuilders {
  /// <summary>
  /// An abstract base class representing a track clip for posing the player.
  /// </summary>
  public abstract class PoseClip : PlayableAsset {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The state that should be updated in the player's Finite State Machine.
    /// </summary>
    [Tooltip("The state that should be updated in the player's Finite State Machine.")]
    [ValueDropdown("GetStateTypes")]
    public string State;

    /// <summary>
    /// The underlying clip asset. This is handed to us from the
    /// PlayerCharacter track, since properties on it like start, end, and
    /// duration can't be accessed otherwise.
    /// </summary>
    [NonSerialized]
    private TimelineClip clip;

    #endregion

    #region Playable API

    /// <summary>
    /// Create a pose asset.
    /// </summary>
    /// <param name="graph">The playable graph this pose will belong to.</param>
    /// <param name="owner">IDK my BFF jill.</param>
    /// <returns>A script playable for the pose</returns>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
      // Use hook method to get the right type of pose info
      ScriptPlayable<PoseInfo> playable = CreateScriptPlayable(graph);
      PoseInfo p = playable.GetBehaviour();

      if (State != null) {
        p.State = Type.GetType(State);
      }

      p.StartTime = (float)clip.start;
      p.EndTime = (float)clip.end;
      p.Duration = (float)clip.duration;

      return playable;
    }

    #endregion

    #region Abstract Methods
    //-------------------------------------------------------------------------
    // Abstract Methods
    //-------------------------------------------------------------------------

    /// <summary>
    /// (Hook Method) Create the script playable for the clip's pose type.
    /// </summary>
    /// <seealso cref="AbsolutePoseClip.CreateScriptPlayable"/>
    /// <seealso cref="RelativePoseClip.CreateScriptPlayable"/>
    public abstract ScriptPlayable<PoseInfo> CreateScriptPlayable(PlayableGraph graph);

    #endregion


    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    public void SetClip(TimelineClip clip) {
      this.clip = clip;
    }
    #endregion

    #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
#if UNITY_EDITOR

    [FoldoutGroup("Default Key Creation")]
    [Button("Create All", ButtonSizes.Large), GUIColor(0.9f, .9f, .9f)]
    public void CreateAllCurves() {
      string[] properties =  {"Position.x", "Position.y", "Position.z", 
                              "Rotation.x", "Rotation.y", "Rotation.z", 
                              "Scale.x", "Scale.y", "Scale.z", 
                              "Active", "Flipped"};
      float[] values = { 0, 0, 0, 
                         0, 0, 0, 
                         1, 1, 1, 
                         1, 0 };

      TimelineEditorTools.CreateEmptyCurves(
        clip: TimelineEditor.selectedClip,
        type: this.GetType(),
        propertyNames: new List<string>(properties),
        defaultValues: new List<float>(values)
      );
    }

    /// <summary>
    /// Creates the clip's curve properties for position if not already present.
    /// </summary>
    [FoldoutGroup("Default Key Creation")]
    [Button, GUIColor(.85f, .85f, .85f)]
    public void CreatePositionCurves() {
      TimelineEditorTools.CreateEmptyCurves(
        clip: TimelineEditor.selectedClip,
        type: this.GetType(),
        propertyNames: new List<string>(new string[] {"Position.x", "Position.y", "Position.z"})
      );
    }

    /// <summary>
    /// Creates the clip's curve properties for rotation if not already present.
    /// </summary>
    [FoldoutGroup("Default Key Creation")]
    [Button, GUIColor(0.8f, .8f, .8f)]
    public void CreateRotationCurves() {
      TimelineEditorTools.CreateEmptyCurves(
        clip: TimelineEditor.selectedClip,
        type: this.GetType(),
        propertyNames: new List<string>(new string[] {"Rotation.x", "Rotation.y", "Rotation.z"})
      );
    }

    /// <summary>
    /// Creates the clip's curve properties for scale if not already present.
    /// </summary>
    [FoldoutGroup("Default Key Creation")]
    [Button, GUIColor(0.75f, .75f, .75f)]
    public void CreateScaleCurves() {
      TimelineEditorTools.CreateEmptyCurves(
        clip: TimelineEditor.selectedClip,
        type: this.GetType(),
        propertyNames: new List<string>(new string[] {"Scale.x", "Scale.y", "Scale.z"}),
        defaultValues: new List<float>(new float[] {1f, 1f, 1f})
      );
    }

    [FoldoutGroup("Default Key Creation")]
    [Button, GUIColor(0.7f, .7f, .7f)]
    public void CreateActiveCurves() {
      TimelineEditorTools.CreateEmptyCurves(
        clip: TimelineEditor.selectedClip,
        type: this.GetType(),
        propertyNames: new List<string>(new string[] {"Active"}),
        defaultValues: new List<float>(new float[] { 1f })
      );
    }

    [FoldoutGroup("Default Key Creation")]
    [Button, GUIColor(0.65f, .65f, .65f)]
    public void CreateFlippedCurves() {
      TimelineEditorTools.CreateEmptyCurves(
        clip: TimelineEditor.selectedClip,
        type: this.GetType(),
        propertyNames: new List<string>(new string[] {"Flipped"}),
        defaultValues: new List<float>(new float[] { 1f })
      );
    }
#endif

    /// <summary>
    /// Get a list of the player's types.
    /// </summary>
    private ValueDropdownList<string> GetStateTypes() {
      ValueDropdownList<string> types = new ValueDropdownList<string>();

      foreach (Type t in PoseClip.GetSubtypesOfTypeInAssembly("HumanBuilders", typeof(PlayerState))) {
        string typeName = t.ToString();

        string[] subs = typeName.Split('.');
        string simpleName = subs[subs.Length - 1];
        string letter = ("" + simpleName[0]).ToUpper();

        string folder = letter + "/" + simpleName;

        types.Add(new ValueDropdownItem<string>(folder, typeName));
      }

      return types;
    }

    /// <summary>
    /// Within a code assembly, searches for all subtypes of the given type.
    /// </summary>
    /// <param name="assemblyName">The name of the C# assembly.</param>
    /// <param name="t">The type to search for.</param>
    /// <returns>The list of types in the assebly that are a subtype of t.</returns>
    private static List<Type> GetSubtypesOfTypeInAssembly(string assemblyName, Type t) {
      List<Type> results = new List<Type>();

      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        if (assembly.FullName.StartsWith(assemblyName)) {
          foreach (Type type in assembly.GetTypes()) {
            if (type.IsSubclassOf(t))
              results.Add(type);
          }
          break;
        }
      }

      return results;
    }
  }
  #endregion
}