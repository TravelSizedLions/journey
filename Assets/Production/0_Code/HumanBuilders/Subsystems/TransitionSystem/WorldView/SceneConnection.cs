
using System;
using HumanBuilders;

namespace TSL.Subsystems.WorldView {
  [Serializable]
  public class SceneConnection {
    public SceneField scene = new SceneField();
    public string spawnPoint;
  }
}