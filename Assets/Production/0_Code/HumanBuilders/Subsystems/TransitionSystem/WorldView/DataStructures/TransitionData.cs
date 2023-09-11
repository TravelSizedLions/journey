using System;
using HumanBuilders;

namespace TSL.Subsystems.WorldView {
  [Serializable]
  public class TransitionData {
    public Guid ID;
    public string Name;
    public SceneField SceneInfo;
    public string TargetSceneName => SceneInfo.SceneName;
    public UnityEngine.Object TargetSceneAsset => SceneInfo.SceneAsset;
    public string SpawnName;

    public TransitionData(Guid id, string name, SceneField scene, string spawn) {
      this.ID = id;
      this.Name = name;
      SceneInfo = scene;
      SpawnName = spawn;
    }

    public TransitionData(Transition transition) {
      ID = transition.ID;
      Name = transition.name;
      SceneInfo = transition.Scene;
      SpawnName = transition.SpawnPoint;
    }

    public TransitionData(TransitionDoor transition) {
      ID = transition.ID;
      Name = transition.name;
      SceneInfo = transition.Scene;
      SpawnName = transition.SpawnName;
    }

    public override bool Equals(object obj) {
      if (obj.GetType() != this.GetType()) {
        return false;
      }

      TransitionData other = (TransitionData) obj;

      return (
        ID == other.ID &&
        Name == other.Name &&
        SceneInfo == other.SceneInfo &&
        SpawnName == other.SpawnName
      );
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }
  }
}