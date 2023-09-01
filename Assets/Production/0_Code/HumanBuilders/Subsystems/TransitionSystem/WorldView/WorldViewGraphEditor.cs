using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HumanBuilders;
using TSL.Editor.SceneUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using XNode;
using XNodeEditor;

namespace TSL.Subsystems.WorldView {
  [CustomNodeGraphEditor(typeof(WorldViewGraph))]
  public class WorldViewGraphEditor : NodeGraphEditor {
    public override bool CanRemove(Node node) {
      return false;
    }

    public override string GetNodeMenuName(Type type) {
      return null;
    }

    public override void AddContextMenuItems(GenericMenu menu, Type compatibleType = null, NodePort.IO direction = NodePort.IO.Input) {
      menu.AddItem(new GUIContent("Re-Analyze Project"), false, AnalyzeGraph);

      base.AddContextMenuItems(menu, compatibleType, direction);
    }

#if UNITY_EDITOR
    public void AnalyzeGraph() {
      if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
        (WorldViewWindow.current.graph as WorldViewGraph).Rebuild();
        List<string> scenes = SceneUtils.GetOpenScenes();

        try {
          // open EVERYTHING
          Dictionary<Scene, SceneNode> pairs = OpenAllScenes();

          // Add transition/spawn data to nodes.
          pairs.Keys.ToList().ForEach(scene => {
            SceneNode sceneNode = pairs[scene];
            List<Transition> transitions = SceneUtils.FindAll<Transition>(scene);
            List<TransitionDoor> doors = SceneUtils.FindAll<TransitionDoor>(scene);
            List<SpawnPoint> spawns = SceneUtils.FindAll<SpawnPoint>(scene);

            transitions.ForEach(transition => sceneNode.AddTransition(transition));
            doors.ForEach(door => sceneNode.AddDoor(door));
            spawns.ForEach(spawn => sceneNode.AddSpawnPoint(spawn));
          });

          // Connect edges.
          pairs.Keys.ToList().ForEach(scene => pairs[scene].Connect());

          // Mark scenes as dirty and save in case of GUID collisions on
          // transitions/spawnpoints. The suckers have a habit of invalidating
          // themselves and regenerating if
          // you're not careful.
          EditorSceneManager.MarkAllScenesDirty();
          EditorSceneManager.SaveOpenScenes();

          // Go back to previously open scenes.
          pairs.Keys.ToList().ForEach(scene => {
            if (scenes.IndexOf(scene.path) < 0) {
              EditorSceneManager.CloseScene(scene, true);
            }
          });
        } catch (Exception e) {
          for (int i = 0; i < EditorSceneManager.loadedSceneCount; i++) {
            EditorSceneManager.CloseScene(EditorSceneManager.GetSceneAt(i), true);
          }

          scenes.ForEach(scene => EditorSceneManager.OpenScene(scene, OpenSceneMode.Additive));
          Debug.LogError(e);
        }
      }
    }

    public Dictionary<Scene, SceneNode> OpenAllScenes() {
      var pairs = new Dictionary<Scene, SceneNode>();

      WorldViewWindow.current.graph.nodes.ForEach(node => {
        var sceneNode = (node as SceneNode);
        if (!pairs.TryAdd(EditorSceneManager.OpenScene(sceneNode.Path, OpenSceneMode.Additive), sceneNode)) {
          Debug.LogWarning($"Couldn't open scene '{sceneNode.Path}'");
        }
      });

      return pairs;
    }
#endif
  }
}