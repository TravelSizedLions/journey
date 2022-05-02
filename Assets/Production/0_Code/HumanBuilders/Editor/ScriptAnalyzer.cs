using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSL.Editor {

  public class ScriptAnalyzer : MonoBehaviour {
    [MenuItem("Window/Analyze Scripts in Scenes")]
    public static void AnalyzeLoadedScenes() {
      for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
        Scene scene = EditorSceneManager.GetSceneAt(i);
        AnalyzeSceneScripts(scene);
      }
    }

    public static void AnalyzeSceneScripts(Scene scene) {
      try {
        using(StreamReader sr = new StreamReader(scene.path)) {
          List<SceneObject> objects = new List<SceneObject>();

          string line;
          while ((line = sr.ReadLine()) != null) {
            if (line.StartsWith("---")) {
              objects.Add(ParseObject(sr));
            }
          }

          Dictionary<string, SceneObject> uniqueScripts = new Dictionary<string, SceneObject>();

          string msg = "";
          // foreach (SceneObject obj in objects) {
          //   msg = msg + obj.ToString() + "\n";
          // }

          foreach (SceneObject obj in objects) {
            if (obj.Type == "MonoBehaviour" && obj.Name != null && !uniqueScripts.ContainsKey(obj.Name)) {
              uniqueScripts.Add(obj.Name, obj);
              msg = msg + obj.ToString() + "\n";
            }
          }

          Debug.Log(msg);
        }
      } catch (Exception e) {
        Debug.Log(e);
      }
    }

    public static SceneObject ParseObject(StreamReader sr) {
      SceneObject obj = new SceneObject();

      string line = sr.ReadLine();

      obj.Type = line.TrimStart().TrimEnd(':');

      if (line.StartsWith("---")) {
        return obj;
      }

      while (!(line = sr.ReadLine()).StartsWith("---")) {
        string trimmed = line.TrimStart();
        string[] parts = trimmed.Split(' ');
        string key = parts[0].TrimEnd(':');
        switch (key) {
          case "m_Name":
            obj.Name = parts[1];
            break;
          case "m_Script":
            obj.FileID = parts.Length > 2 ? parts[2].TrimEnd(',') : "";
            obj.ScriptGUID = parts.Length > 4 ? parts[4].TrimEnd(',') : "";
            obj.ScriptType = parts.Length > 6 ? parts[6].TrimEnd('}') : "";
            break;
          default:
            break;
        }

        obj.Lines.Add(line);
      }

      return obj;
    }
  }

  public class SceneObject {
    public List<string> Lines;
    public string Type;
    public string Name;
    public string FileID;
    public string ScriptGUID;
    public string ScriptType;
    public SceneObject() {
      Lines = new List<string>();
    }

    public override string ToString() {
      return string.Format(
        "{0}: {1}",
        Name,
        ScriptGUID
      );
    }
  }
}