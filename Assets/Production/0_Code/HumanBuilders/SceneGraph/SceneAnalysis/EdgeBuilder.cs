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
        Debug.Log("Starting");
        graph.Nodes.ForEach(node => {
          EditorSceneManager.OpenScene(node.Path);
          List<TransitionDoor> transitions = SceneUtils.FindAll<TransitionDoor>();
          transitions.ForEach(transition => node.AddTransition(transition));
          SceneUtils.FindAll<SpawnPoint>().ForEach(spawn => node.AddSpawnPoint(spawn));
          // EditorSceneManager.MarkAllScenesDirty();
          // EditorSceneManager.SaveOpenScenes();
        });
      }
    }
  }

}

#endif