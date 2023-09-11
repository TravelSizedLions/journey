#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TSL.Editor.SceneUtilities;

namespace TSL.Subsystems.WorldView {
  [Serializable]
  public class ProjectSceneData {
    public List<SceneData> Scenes = new List<SceneData>();

    public List<string> Paths => Scenes.ConvertAll(s => s.Path);

    public void Construct() {
      SceneUtils.GetActiveScenesInBuild().ForEach(path => {
        SceneData data = new SceneData();
        data.Construct(path);
        Scenes.Add(data);
      });
    }

    public SceneData this[string path] {
      get {
        int index = Scenes.FindIndex(s => s.Path == path);
        if (index >= 0) {
          return Scenes[index];
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
      var copy = Scenes.ConvertAll(s => s);
      copy.ForEach(scene => {
        if (!buildScenes.Contains(scene.Path)) {
          Scenes.Remove(scene);
          changed = true;
        }
      });
      return changed;
    }

    public bool AddNewScenes(List<string> buildScenes) {
      bool changed = false;
      buildScenes.ForEach(path => {;
        if (this[path] == null) {
          SceneData data = new SceneData();
          data.Construct(path);
          Scenes.Add(data);
          changed = true;
        }
      });
      return changed;
    }
  }
}
#endif
