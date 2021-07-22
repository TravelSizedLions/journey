using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace HumanBuilders {
  [CreateAssetMenu(fileName = "New Game Settings", menuName = "Journey/Game Settings")]
  public class JourneySettings : ScriptableObject {
    private static JourneySettings _inst;
    public static JourneySettings Instance {
      get {
        if (_inst == null) {
          _inst = Resources.Load<JourneySettings>("quest_settings");
        }

        return _inst;
      }
    }

    public Narrator Narrator;

    public QuestGraph MainQuest;

    public static List<string> FindSettingsFiles()  {
      #if UNITY_EDITOR
      List<string> assets = new List<string>();
      string[] guids = AssetDatabase.FindAssets(string.Format("t:{0}", typeof(JourneySettings)));
      for (int i = 0; i < guids.Length; i++) {
        string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
        JourneySettings asset = AssetDatabase.LoadAssetAtPath<JourneySettings>(assetPath);
        if (asset != null) {
          assets.Add(asset.name);
        }
      }
      return assets;
      #else
      return null;
      #endif
    }
  }
}