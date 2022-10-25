using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using FJSON;
using System;
using HumanBuilders.Attributes;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
using UnityEditor.Callbacks;
using UnityEditor;
#endif

namespace HumanBuilders {

  public class SettingsMenu : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Constants
    //-------------------------------------------------------------------------
    public const string SETTINGS_PATH = "Settings/basic_settings.json";

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The prefab to use to display available settings.
    /// </summary>
    [Tooltip("The prefab to use to display available settings.")]
    [AutoTable(typeof(SettingsEntry))]
    public List<SettingsEntry> Settings;

    /// <summary>
    /// The game object to place settings menu items into.
    /// </summary>
    [Tooltip("The game object to place setting menu items into.")]
    public Transform SettingsContainer;

    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void OnAwake() {
      foreach (Transform child in SettingsContainer) {
        Destroy(child.gameObject);
      }
    }

    private void OnEnable() {
      // string filePath = Path.Combine(Application.persistentDataPath, SETTINGS_PATH);
      
      // // Editor: Just create new map data.
      // // Player: Copy map data from working directory to persistent path.
      // #if UNITY_EDITOR
      // if (!File.Exists(filePath)) {
      //   // GenerateMapData();
      //   // Probably Create settings file instead
      // }
      // #else
      // if (!File.Exists(filePath)) {
      //   Debug.Log("HumanBuilders: Setting default settings");

      //   string installPath = Path.Combine(Directory.GetCurrentDirectory(), SETTINGS_PATH);
      //   new FileInfo(filePath).Directory?.Create();
      //   File.Copy(installPath, filePath, true);
      // }
      // #endif

      // StreamReader file = new StreamReader(filePath);
      // string json = file.ReadToEnd();
      // file.Close();

      foreach (SettingsEntry entry in Settings) {
        Debug.Log(entry.DisplayName);
        GameObject go = Instantiate(entry.Prefab, SettingsContainer);
        VolumeSettingControl control = go.GetComponentInChildren<VolumeSettingControl>(true);
        control?.SetDisplayName(entry.DisplayName);
        control?.LoadSetting(entry.SettingFile, entry.SettingName);
      }
    }

    private void OnDisable() {
      foreach (Transform child in SettingsContainer) {
        Destroy(child.gameObject);
      }
    }
  }
}