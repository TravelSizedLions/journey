namespace Storm.Subsystems.Save {
  /// <summary>
  /// A list of keys that can be used to store in-game information.
  /// </summary>
  public static class Keys {
    /// <summary>
    /// The key for storing the "current" dialog on objects that can trigger a dialog.
    /// </summary>
    public const string CURRENT_DIALOG="_current_dialog";


    /// <summary>
    /// The key for storing whether or not a self-destructing object should stay
    /// destroyed.
    /// </summary>
    public const string KEEP_DESTROYED="_keep_destroyed";
  }
}