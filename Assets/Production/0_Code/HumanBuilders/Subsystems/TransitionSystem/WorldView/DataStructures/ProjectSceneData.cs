using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TSL.Editor.SceneUtilities;

namespace TSL.Subsystems.WorldView {
  [Serializable]
  public class ProjectSceneData {
    private Dictionary<string, SceneData> scenes = new Dictionary<string, SceneData>();

    public List<string> Paths => scenes.Keys.ToList();

    public List<SceneData> Scenes => scenes.Values.ToList();

    public void Construct() {
      SceneUtils.GetActiveScenesInBuild().ForEach(path => {
        SceneData data = new SceneData();
        data.Construct(path);
        scenes.Add(path, data);
      });
    }

    public SceneData this[string path] {
      get {
        if (scenes.ContainsKey(path)) {
          return scenes[path];
        }

        return null;
      }
    }

    public bool Sync() {
      List<string> buildScenes = SceneUtils.GetActiveScenesInBuild();
      bool changed = false;
      changed |= RemoveUnusedScenes(buildScenes);
      changed |= AddNewScenes(buildScenes);
      return changed;
    }

    public bool RemoveUnusedScenes(List<string> buildScenes) {
      bool changed = false;
      Paths.ForEach(path => {
        if (!buildScenes.Contains(path)) {
          scenes.Remove(path);
          changed = true;
        }
      });
      return changed;
    }

    public bool AddNewScenes(List<string> buildScenes) {
      bool changed = false;
      buildScenes.ForEach(path => {;
        if (!scenes.ContainsKey(path)) {
          SceneData data = new SceneData();
          data.Construct(path);
          scenes.Add(path, data);
          changed = true;
        }
      });
      return changed;
    }

    // public bool UpdateCurentScenes() {
    //   bool changed = false;
    //   Scenes.ForEach(data => {
    //     changed |= data.Sync(data.Path);
    //   });
    //   return changed;
    // }
  }
}