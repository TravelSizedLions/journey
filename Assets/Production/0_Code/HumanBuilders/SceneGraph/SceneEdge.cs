using System;

namespace TSL.SceneGraphSystem {
  [Serializable]
  public class SceneEdge {
    public SceneNode ToNode {get => node;}
    private SceneNode node;

    public string ToSpawn {get => spawn;}
    private string spawn;
    public SceneEdge(SceneNode toNode, string spawnName) {
      this.node = toNode;
      spawn = spawnName;
    }
  }
}
