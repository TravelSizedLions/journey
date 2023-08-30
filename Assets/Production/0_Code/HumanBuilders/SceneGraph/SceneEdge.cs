using System;
using Sirenix.OdinInspector;

namespace TSL.SceneGraphSystem {
  [Serializable]
  public class SceneEdge {
    public SceneNode ToNode {get => node;}

    [ShowInInspector]
    [ReadOnly]
    [LabelText("To Node")]
    private SceneNode node;

    public string ToSpawn {get => spawn;}

    [ShowInInspector]
    [ReadOnly]
    [LabelText("To Spawn")]
    private string spawn;

    public SceneEdge(SceneNode toNode, string spawnName) {
      this.node = toNode;
      spawn = spawnName;
    }
  }
}
