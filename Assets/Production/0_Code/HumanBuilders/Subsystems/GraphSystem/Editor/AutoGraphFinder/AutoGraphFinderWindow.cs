using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders.Graphing.Editor {
  public class AutoGraphFinderWindow : EditorWindow {
    private string variableName = "";

    [MenuItem("Journey/Graphing/Finder")]
    public static void Open() {
      GetWindow<AutoGraphFinderWindow>("AutoGraph Finder");
    }

    public void OnGUI() {
      GUILayout.Space(20);
      GUILayout.Label("VSave", EditorStyles.boldLabel);
      variableName = EditorGUILayout.TextField("Variable Name", variableName);

      if(GUILayout.Button("Find")) {
        Debug.Log("Test2");
      }
    }
  }
}