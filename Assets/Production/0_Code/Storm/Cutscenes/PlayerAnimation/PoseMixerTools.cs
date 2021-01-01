
using Storm.Characters.Player;
using UnityEngine;

namespace Storm.Cutscenes {
  /// <summary>
  /// Tools to help mix Player pose information.
  /// </summary>
  public static class PoseMixerTools {

    /// <summary>
    /// Whether or not a pose clip is meant for absolute positioning.
    /// </summary>
    /// <param name="template">The pose information</param>
    /// <returns>True if the pose information is for an AbsolutePose clip</returns>
    public static bool IsAbsolute(PoseInfo template) => template.GetType() == typeof(AbsolutePoseInfo);
    
    /// <summary>
    /// Whether or not a pose clip is meant for relative positioning.
    /// </summary>
    /// <param name="template">The pose information</param>
    /// <returns>True if the pose information is for a RelativePose clip</returns>
    public static bool IsRelative(PoseInfo template) => template.GetType() == typeof(RelativePoseInfo);
    
    /// <summary>
    /// Apply the pose of an absolute clip.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="snapshot">A snapshot of player character information</param>
    /// <param name="pose">The pose to apply</param>
    /// <param name="weight">(optional) The weighting on the pose, used to
    /// interpolate this pose with another if necessary. Default: 1.</param>
    /// <param name="updateVirtualSnapshot">(optional) Whether or not to update the mixer's virtual snapshot
    /// of the player character. When mixing multiple absolute poses, this
    /// action can be saved for the last pose applied. Default: true.</param>
    public static void MixAbsolute(PlayerCharacter player, PlayerSnapshot snapshot, AbsolutePoseInfo pose, float weight = 1f) {
      Vector3 pos = snapshot.Position;
      Vector3 rot = snapshot.Rotation;
      Vector3 scl = snapshot.Scale;

      player.transform.position = (pose.Position-pos)*weight + pos;
      player.transform.eulerAngles = (pose.Rotation-rot)*weight + rot;
      player.transform.localScale = (pose.Scale-scl)*weight + scl;
    }

    /// <summary>
    /// Apply the pose of a relative clip.
    /// </summary>
    /// <param name="player">The player character</param>
    /// <param name="snapshot">A snapshot of player character information</param>
    /// <param name="pose">The pose to apply</param>
    /// <param name="weight">(optional) The weighting on the pose, used to
    /// interpolate this pose with another if necessary. Default: 1.</param>
    public static void MixRelative(PlayerCharacter player, PlayerSnapshot snapshot, RelativePoseInfo pose, float weight = 1f) {
      player.transform.position += pose.Position*weight;
      player.transform.eulerAngles += pose.Rotation*weight;
      player.transform.localScale = (pose.Scale-snapshot.Scale)*weight + snapshot.Scale;
    }
  }
}