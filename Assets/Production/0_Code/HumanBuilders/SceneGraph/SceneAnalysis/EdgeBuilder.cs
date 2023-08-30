#if UNITY_EDITOR
using TSL.Editor.SceneUtilities;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using HumanBuilders;
using System.Collections.Generic;

namespace TSL.SceneGraphSystem {
  public static class EdgeBuilder {
    
    public static void BuildEdges(SceneGraph graph) {
      if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
        graph.Nodes.ForEach(node => {
          EditorSceneManager.LoadScene(node.Name);
          List<Transition> transitions = SceneUtils.FindAll<Transition>();
          transitions.ForEach(transition => node.AddTransition(transition));
          SceneUtils.FindAll<SpawnPoint>().ForEach(spawn => node.AddSpawnPoint(spawn));
        });
      }
    }
  }

}

#endif