#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace HumanBuilders.Graphing.Editor {
  public class ProjectReport<R> : Report where R : SceneReport {
    public List<R> SceneReports { get => sceneReports; }
    protected List<R> sceneReports;

    public ProjectReport(params object[] extraParams) {
      sceneReports = new List<R>();
      GetAllScenesInBuild().ForEach(
        (string path) => {
          var report = (R)Activator.CreateInstance(typeof(R), path, extraParams);
          sceneReports.Add(report);
        }
      );
    }

    protected override string BuildMessage() {
      string message = "";

      foreach (var report in sceneReports) {
        if (report.HasMessage) {
          string[] parts = report.ScenePath.Split('/');
          string fileName = parts[parts.Length-1].Split('.')[0];

          message += string.Format("--- {0} ---\n", fileName);
          message += report.Message + "\n";
        }
      }

      return message;
    }

    protected List<string> GetAllScenesInBuild() {
      List<string> scenes = new List<string>();

      int sceneCount = SceneManager.sceneCountInBuildSettings;
      for (int i = 0; i < sceneCount; i++) {
        string path = SceneUtility.GetScenePathByBuildIndex(i);
        if (!string.IsNullOrEmpty(path)) {
          scenes.Add(path);
        }
      }
      return scenes;
    }
  }
}
#endif