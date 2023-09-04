using System;
using HumanBuilders;

namespace TSL.Subsystems.WorldView {
  [Serializable]
  public class TransitionData {
    public Guid ID => id;
    private Guid id;

    public string Name => name;
    private string name;

    private SceneField sceneInfo;
    public string TargetSceneName => sceneInfo.SceneName;
    public UnityEngine.Object TargetSceneAsset => sceneInfo.SceneAsset;

    public string SpawnName => spawnName;
    private string spawnName;

    public TransitionData(Guid id, string name, SceneField scene, string spawn) {
      this.id = id;
      this.name = name;
      sceneInfo = scene;
      spawnName = spawn;
    }

    public TransitionData(Transition transition) {
      id = transition.ID;
      name = transition.name;
      sceneInfo = transition.Scene;
      spawnName = transition.SpawnPoint;
    }

    public TransitionData(TransitionDoor transition) {
      id = transition.ID;
      name = transition.name;
      sceneInfo = transition.Scene;
      spawnName = transition.SpawnName;
    }

    public override bool Equals(object obj) {
      if (obj.GetType() != this.GetType()) {
        return false;
      }

      TransitionData other = (TransitionData) obj;

      return (
        id == other.id &&
        name == other.name &&
        sceneInfo == other.sceneInfo &&
        spawnName == other.spawnName
      );
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }
  }
}