using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Extensions;
using UnityEngine;

namespace Storm.Subsystems.Saving {

  /// <summary>
  /// The top level Facade for data management.
  /// </summary>
  public class DataManager : Singleton<DataManager> {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The save files.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private List<SaveFile> slots;

    /// <summary>
    /// The name of the folder to save game data out to.
    /// </summary>
    [Tooltip("The name of the folder to save game data out to.")]
    public string FolderName;

    /// <summary>
    /// The currently loaded savefile for the game.
    /// </summary>
    private SaveFile activeFile;

    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private new void Awake() {
      base.Awake();

      slots = new List<SaveFile>();
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Saves game data for a certain save file out to disk.
    /// </summary>
    /// <param name="filename">The name of the save file to save.</param>
    public bool Save(string filename) {
      SaveFile file = slots.Find((SaveFile slot) => slot.Name == filename);

      if (file != null) {
        return file.Save();
      }

      return false;
    }


    /// <summary>
    /// Loads all game data for a certain save file from disk.
    /// </summary>
    /// <param name="filename">The name of the save file to load.</param>
    public bool Load(string filename) {
      SaveFile file = slots.Find((SaveFile slot) => slot.Name == filename);

      if (file != null) {
        return file.Load();
      }

      return false;
    }

    /// <summary>
    /// Delete a slot with a given name.
    /// </summary>
    /// <param name="filename">The name of the save file to delete.</param>
    public void Delete(string filename) {
      SaveFile file = slots.Find((SaveFile slot) => slot.Name == filename);

      if (file != null) {
        file.DeleteFolder();
        slots.Remove(file);
      }
    }

    /// <summary>
    /// Create a save slot.
    /// </summary>
    /// <param name="filename">The name of the save file to create.</param>
    public void CreateSaveFile(string filename) {
      SaveFile file = new SaveFile(FolderName, filename);
      slots.Add(file);
    }
    #endregion
  }
}