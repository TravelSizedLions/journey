
using System;
using HumanBuilders;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace TSL.Subsystems.WorldView {
  [Serializable]
  public class TransitionData {

    private SceneField scene = new SceneField();
    private string spawnPoint;

    public TransitionData(Scene targetScene, string targetSpawn) {
      scene.SceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(targetScene.path);
      spawnPoint = targetSpawn;
    }
  }
}