namespace Storm.Subsystems.Save {
  /// <summary>
  /// A list of keys that can be used to store in-game information.
  /// </summary>
  public static class Keys {
    /// <summary>
    /// The key for storing the "current" dialog on objects that can trigger a dialog.
    /// </summary>
    public const string CURRENT_DIALOG="current_dialog";

    /// <summary>
    /// The key for storing whether or not a self-destructing object should stay
    /// destroyed.
    /// </summary>
    public const string KEEP_DESTROYED="keep_destroyed";

    /// <summary>
    /// The key for storing the animation state for an object.
    /// </summary>
    public const string ANIM_STATE="anim_state";

    /// <summary>
    /// The key for storing an animation int parameter.
    /// </summary>
    public const string ANIM_INT="anim_int";

    /// <summary>
    /// The key for storing an animation boolean parameter.
    /// </summary>
    public const string ANIM_BOOL="anim_bool";

    /// <summary>
    /// The key for storing an animation float parameter.
    /// </summary>
    public const string ANIM_FLOAT="anim_float";

    /// <summary>
    /// The key for storing an animation trigger parameter.
    /// </summary>
    public const string ANIM_TRIGGER="anim_trigger";

    /// <summary>
    /// The key for storing the position of an object.
    /// </summary>
    public const string POSITION="position";

    /// <summary>
    /// The key for storing whether or not a script is enabled.
    /// </summary>
    public const string ENABLED="enabled";
  }
}