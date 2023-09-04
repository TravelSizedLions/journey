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
      SceneUtils.GetActiveScenesInBuild().ForEach(path => scenes.Add(path, new SceneData(path)));
    }

    public SceneData this[string path] {
      get {
        if (scenes.ContainsKey(path)) {
          return scenes[path];
        }

        return null;
      }
    }
  }
}