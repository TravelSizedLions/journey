namespace HumanBuilders {
  /// <summary>
  /// How to handle the end of a timeline.
  /// 
  /// <list type="bullet">
  /// <item> Resume - Resume normal play from where the timeline put the player. </item>
  /// <item> Freeze - Freeze the player in place where the timeline put them.</item>
  /// <item> Revert - Move the player back to their original state prior to the timeline.</item>
  /// </list>
  /// </summary>
  public enum OutroSetting {
    /// <summary> Resume normal play from where the timeline put the player. </summary>
    Resume,

    /// <summary> Freeze the player in place where the timeline put them.</summary>
    Freeze,

    /// <summary> Move the player back to their original state prior to the timeline.</summary>
    Revert
  }
}