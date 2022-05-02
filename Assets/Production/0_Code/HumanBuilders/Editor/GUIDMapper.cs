using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TSL.Editor {

  public class ScriptInfo {
    public string Name;
    public string GUID;
    public ScriptInfo(string name, string guid) {
      Name = name;
      GUID = guid;
    }

    public override string ToString() {
      return string.Format(
        "{0} (GUID: {1})",
        Name,
        GUID
      );
    }
  }

  public class GUIDMapper : MonoBehaviour {
    const string PRE_PATH = "J:\\0_journey\\Assets\\Production\\0_Code\\HumanBuilders\\Subsystems\\GraphSystem";
    const string POST_PATH = "J:\\1_packages\\autograph\\Packages\\TSL.Autograph";
    
    [MenuItem("Window/Show GUIDs")]
    public static void ShowGUIDs() {
      Dictionary<string, ScriptInfo> preList = CollectGUIDsAtPath(PRE_PATH);
      Dictionary<string, ScriptInfo> postList = CollectGUIDsAtPath(POST_PATH);

      string msg = "---- Before ----\n";
      foreach (ScriptInfo info in preList.Values) {
        msg += info.Name + " - " + info.GUID + "\n";
      }

      msg += string.Format("{0} items\n\n", preList.Count);

      msg += "---- After ----\n";
      foreach (ScriptInfo info in postList.Values) {
        msg += info.Name + " - " + info.GUID + "\n";
      }

      msg += string.Format("{0} items\n\n", postList.Count);
      Debug.Log(msg);
    }


    [MenuItem("Window/Remap GUIDs")]
    public static void RemapGUIDs() {
      Dictionary<string, ScriptInfo> preList = CollectGUIDsAtPath(PRE_PATH);
      List<string> missing = ModifyGUIDsAtPath(POST_PATH, preList);

      string msg = "Missing Scripts:";
      foreach (string path in missing) {
        msg += path + "\n";
      }

      Debug.Log(msg);
    }

    public static Dictionary<string, ScriptInfo> CollectGUIDsAtPath(string path) {
      Dictionary<string, ScriptInfo> metaFiles = new Dictionary<string, ScriptInfo>();
      // List<ScriptInfo> metaFiles = new List<ScriptInfo>();

      foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)) {
        if (file.EndsWith(".meta")) {
          ScriptInfo info = ExtractGUIDFromMeta(file);
          if (info != null) {
            metaFiles.Add(info.Name, info);
          }
        }
      }

      // string msg = "";
      // foreach (string metaFile in metaFiles.Keys) {
      //   msg += metaFiles[metaFile].ToString() + "\n";
      // }

      // Debug.Log(msg);

      return metaFiles;
    }

    public static List<string> ModifyGUIDsAtPath(string path, Dictionary<string, ScriptInfo> metaFiles) {
      List<string> missingScripts = new List<string>();

      int count = 0;
      foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories)) {
        if (file.EndsWith(".meta")) {
          
          if(!TryModifyFile(file, metaFiles)) {
            missingScripts.Add(file);
          } else {
            count ++;
          }
        }
      }

      Debug.Log(string.Format("Modified {0} files.", count));
      return missingScripts;
    }

    public static bool TryModifyFile(string filePath, Dictionary<string, ScriptInfo> metaFiles) {
      string[] parts = filePath.Split('\\');
      string name = parts[parts.Length-1];

      if (metaFiles.ContainsKey(name)) {
        ScriptInfo info = metaFiles[name];
        string line = string.Format("guid: {0}", info.GUID);
        // Debug.Log(string.Format("{0} - \"{1}\"", info.Name, line));

        string[] lines = File.ReadAllLines(filePath);
        lines[1] = line;
        File.WriteAllLines(filePath, lines);

        return true;
      }

      return false;
    }

    public static ScriptInfo ExtractGUIDFromMeta(string filePath) {
      // J:\0_journey\Assets\Production\0_Code\HumanBuilders\Subsystems\GraphSystem\AutoGraph.cs.meta
      
      string[] parts = filePath.Split('\\');
      string name = parts[parts.Length-1];
      try {
        using (StreamReader sr = File.OpenText(filePath)) {
          sr.ReadLine();
          string guidLine = sr.ReadLine();
          string guid = guidLine.Split(' ')[1];

          return new ScriptInfo(name, guid);
        }
      } catch (Exception e) {
        Debug.Log(e);
      }

      return null;
    }


    // public static void AnalyzeLoadedScenes() {
    //   for (int i = 0; i < EditorSceneManager.sceneCount; i++) {
    //     Scene scene = EditorSceneManager.GetSceneAt(i);
    //     AnalyzeSceneScripts(scene);
    //   }
    // }

    // public static void AnalyzeSceneScripts(Scene scene) {
    //   try {
    //     using(StreamReader sr = new StreamReader(scene.path)) {
    //       List<SceneObject> objects = new List<SceneObject>();

    //       string line;
    //       while ((line = sr.ReadLine()) != null) {
    //         if (line.StartsWith("---")) {
    //           objects.Add(ParseObject(sr));
    //         }
    //       }

    //       Dictionary<string, SceneObject> uniqueScripts = new Dictionary<string, SceneObject>();

    //       string msg = "";
    //       // foreach (SceneObject obj in objects) {
    //       //   msg = msg + obj.ToString() + "\n";
    //       // }

    //       foreach (SceneObject obj in objects) {
    //         if (obj.Type == "MonoBehaviour" && obj.Name != null && !uniqueScripts.ContainsKey(obj.Name)) {
    //           uniqueScripts.Add(obj.Name, obj);
    //           msg = msg + obj.ToString() + "\n";
    //         }
    //       }

    //       Debug.Log(msg);
    //     }
    //   } catch (Exception e) {
    //     Debug.Log(e);
    //   }
    // }

  //   public static SceneObject ParseObject(StreamReader sr) {
  //     SceneObject obj = new SceneObject();

  //     string line = sr.ReadLine();

  //     obj.Type = line.TrimStart().TrimEnd(':');

  //     if (line.StartsWith("---")) {
  //       return obj;
  //     }

  //     while (!(line = sr.ReadLine()).StartsWith("---")) {
  //       string trimmed = line.TrimStart();
  //       string[] parts = trimmed.Split(' ');
  //       string key = parts[0].TrimEnd(':');
  //       switch (key) {
  //         case "m_Name":
  //           obj.Name = parts[1];
  //           break;
  //         case "m_Script":
  //           obj.FileID = parts.Length > 2 ? parts[2].TrimEnd(',') : "";
  //           obj.ScriptGUID = parts.Length > 4 ? parts[4].TrimEnd(',') : "";
  //           obj.ScriptType = parts.Length > 6 ? parts[6].TrimEnd('}') : "";
  //           break;
  //         default:
  //           break;
  //       }

  //       obj.Lines.Add(line);
  //     }

  //     return obj;
  //   }
  // }
  }
}