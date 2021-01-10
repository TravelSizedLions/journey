
namespace HumanBuilders {

  /// <summary>
  /// A temporary save slot used to test out levels In-Editor without worrying
  /// about clearing out save data every time.
  /// </summary>
  public class TempSaveSlot : Singleton<TempSaveSlot> {

#if UNITY_EDITOR
    /// <summary>
    /// Whether or not VSave has already been initialized.
    /// </summary>
    private static bool initialized = false;

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private new void Awake() {
      base.Awake();

      if (!initialized) {
        VSave.CreateSlot("temp");
        VSave.ChooseSlot("temp");
        initialized = true;
      }
    }

    private new void OnApplicationQuit() {
      base.OnApplicationQuit();
      VSave.Reset();
    }

    #endregion
#endif

  }
}