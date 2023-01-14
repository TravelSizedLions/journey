#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace HumanBuilders.Graphing.Editor {
  public class AutoGraphFinderWindow : EditorWindow {
    private string variableName = "";
    private bool searchAcrossProject = false;
    private Vector2 scrollPosition;

    private SceneVariableSearchReport sceneReport;
    private ProjectVariableSearchReport projReport;

    [MenuItem("Journey/Graphing/Finder")]
    public static void Open() {
      GetWindow<AutoGraphFinderWindow>("AutoGraph Finder");
    }

    public void OnGUI() {
      GUILayout.Space(20);
      GUILayout.Label("Search For Variable References", EditorStyles.boldLabel);
      variableName = EditorGUILayout.TextField("Variable Name", variableName);
      searchAcrossProject = EditorGUILayout.Toggle("Search Across Project", searchAcrossProject);

      if(GUILayout.Button("Search")) {
        CalculateResults();
      }

      ShowResults();
    }

    private void CalculateResults() {
      if (searchAcrossProject) {
        projReport = new ProjectVariableSearchReport(variableName);
        sceneReport = null;
      } else {
        sceneReport = new SceneVariableSearchReport(SceneManager.GetActiveScene().path, variableName);
        projReport = null;
      }
    }

    private void ShowResults() {
      scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
      if (projReport != null) {
        foreach (var sReport in projReport.SceneReports) {
          ShowSceneResults(sReport);
        }
      } else if (sceneReport != null) {
        ShowSceneResults(sceneReport);
      }
      EditorGUILayout.EndScrollView();
    }

    private void ShowSceneResults(SceneVariableSearchReport report) {
      if (report.ReferencesVariable) {
        GUILayout.Label(report.ScenePath);
        foreach (var gReport in report.GraphReports) {
          if (gReport.ReferencesVariable) {
            GUILayout.Label(string.Format(" - {0}", gReport.GraphName));
            foreach (var r in gReport.References) {
              GUILayout.Label(string.Format("    - {0}", r));
            }
          }
        }
      }
    }
  }
}

#endif