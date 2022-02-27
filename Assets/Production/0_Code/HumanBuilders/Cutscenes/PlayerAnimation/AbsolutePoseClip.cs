using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Timeline;
using UnityEditorInternal;
#endif

namespace HumanBuilders {
  /// <summary>
  /// A class representing a player pose clip for animating the player character's transform relative to the origin.
  /// </summary>
  /// <seealso cref="RelativePoseClip" />
  /// <seealso cref="PlayerCharacterTrack" />
  /// <seealso cref="AbsolutePoseInfo" />
  public class AbsolutePoseClip : PoseClip {

    /// <summary>
    /// The template for absolute posing clips.
    /// </summary>
    /// <remarks>
    /// While it's tempting to pull up this variable and it's sister variable in
    /// RelativePose.cs into Pose.cs, these templates are the only way for the
    /// track mixer to identify which type of clips it's mixing together (see
    /// <see cref="PlayerCharacterTrackMixer.MixMultiple"/>).
    /// </remarks>
    public AbsolutePoseInfo template;

    /// <summary>
    /// Create the script playable for the pose type.
    /// </summary>
    public override ScriptPlayable<PoseInfo> CreateScriptPlayable(PlayableGraph graph)  => ScriptPlayable<PoseInfo>.Create(graph, template);


    #if UNITY_EDITOR
    #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
    
    [FoldoutGroup("Key From Transform")]
    [Button("All", ButtonStyle.FoldoutButton), GUIColor(.85f, .85f, .85f)]
    public void CreateAllKeysFromTransform(Transform transform) {

      TimelineClip clip = TimelineEditor.selectedClip;
      if (clip.curves != null) {
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(clip.curves);
        
      }
    }

    [FoldoutGroup("Key From Transform")]
    [Button("Position", ButtonStyle.FoldoutButton), GUIColor(.8f, .8f, .8f)]
    public void CreatePositionKeysFromTransform(Transform transform) {
      List<string> properties = new List<string>(new string[] {"Position.x", "Position.y", "Position.z"});
      float[] values = new float[] { transform.position.x, transform.position.y, transform.position.z};
      EditorCurveBinding[] propBindings = new EditorCurveBinding[3];
      TimelineClip clip = TimelineEditor.selectedClip;

      if (clip.curves != null) {
        foreach (EditorCurveBinding binding in AnimationUtility.GetCurveBindings(clip.curves)) {
          if (properties.Contains(binding.propertyName)) {
            propBindings[properties.IndexOf(binding.propertyName)] = binding;
          }
        }
      
        
        for (int i = 0; i < propBindings.Length; i++) {
          if (string.IsNullOrEmpty(propBindings[i].propertyName)) {
            propBindings[i] = TimelineEditorTools.CreateEmptyCurve(clip, this.GetType(), properties[i]);
          }

          AnimationCurve curve = AnimationUtility.GetEditorCurve(clip.curves, propBindings[i]);
          
          float time = (float)(TimelineEditor.masterDirector.time - clip.start);
          if (curve.AddKey(time, values[i]) == -1) {

            for (int j = 0; j < curve.keys.Length; j++) {
              if (curve.keys[j].time == time) {
                curve.RemoveKey(j);
                break;
              }
            }

            curve.AddKey(time, values[i]);
          }

          clip.curves.SetCurve("", this.GetType(), properties[i], curve);
        }

        TimelineEditor.masterDirector.RebuildGraph();
        TimelineEditor.inspectedDirector.RebuildGraph();
        TimelineEditor.Refresh(RefreshReason.ContentsModified);
        UnityEditorInternal.InternalEditorUtility.RepaintAllViews();
      }
    }

    // TODO: Fill out stubs!

    [FoldoutGroup("Key From Transform")]
    [Button("Rotation", ButtonStyle.FoldoutButton), GUIColor(.75f, .75f, .75f)]
    public void CreateRotationKeysFromTranform(Transform transform) {

    }

    [FoldoutGroup("Key From Transform")]
    [Button("Scale", ButtonStyle.FoldoutButton), GUIColor(.7f, .7f, .7f)]
    public void CreateScaleKeysFromTransform(Transform transform) {

    }

    private void CreatePositionKeys(TimelineClip clip) {

    }

    private void CreateRotationKeys(TimelineClip clip) {

    }

    private void CreateScaleKeys(TimelineClip clip) {

    }

    #endregion
    #endif
  }
}