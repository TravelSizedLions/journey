using System.Collections;
using System.Collections.Generic;
using HumanBuilders;
using HumanBuilders.Attributes;
using HumanBuilders.Graphing;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.GUID;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using TSL.Editor.SceneUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


namespace TSL.Subsystems.WorldView {
  [NodeWidth(600)]
  public class SceneNode : AutoNode {
    public SceneField Scene { get => scene; }
    private SceneField scene;

    public string Path { get => path; }
    private string path;

    public string Key { get => guid; }
    private string guid;

    /// <summary>
    /// Maps each Transition GameObject's GUID to its name
    /// </summary>
    [FoldoutGroup("Debug", false)]
    [ReadOnly]
    [AutoTable(typeof(SceneComponentRef))]
    public List<SceneComponentRef> transitions = new List<SceneComponentRef>();

    /// <summary>
    /// Maps each TransitionDoor GameObject's GUID to its name
    /// </summary>
    [FoldoutGroup("Debug", false)]
    [ReadOnly]
    [AutoTable(typeof(SceneComponentRef))]
    public List<SceneComponentRef> doors = new List<SceneComponentRef>();

    /// <summary>
    /// Maps each spawnpoint GameObject's GUID to its name
    /// </summary>
    [FoldoutGroup("Debug", false)]
    [ReadOnly]
    [AutoTable(typeof(SceneComponentRef))]
    public List<SceneComponentRef> spawnPoints = new List<SceneComponentRef>();

#if UNITY_EDITOR
    public void Construct(string scenePath) {
      guid = GUID.Generate().ToString();
      path = scenePath;
      scene = new SceneField();
      scene.SceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(path);
      name = path.Split('/') [path.Split('/').Length - 1];
    }

    [ContextMenu("Re-Analyze Scene")]
    public void Analyze() {
      if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
        Clear();
        EditorSceneManager.OpenScene(path);
        SceneUtils.FindAll<Transition>().ForEach(transition => AddTransition(transition));
        SceneUtils.FindAll<TransitionDoor>().ForEach(door => AddDoor(door));
        SceneUtils.FindAll<SpawnPoint>().ForEach(spawn => AddSpawnPoint(spawn));
      }
    }

    [ContextMenu("Find Transitions")]
    public void Connect() {
      Scene scene = EditorSceneManager.GetSceneByPath(path);
      if (!scene.IsValid() || !scene.isLoaded) {
        return;
      }

      transitions.ForEach(t => {
        Debug.Log($"NAME: {t.Name}");
        Transition transition = SceneUtils.Find<Transition>(scene, t.Name);
        
        SceneNode otherNode = (SceneNode) graph.nodes.Find(node => (node as SceneNode).NodeName == (transition.Scene.SceneName + ".unity"));
        if (otherNode == null) {
          Debug.LogWarning($"Couldn't find scene {transition.Scene.SceneName}");
          return;
        }

        var entrance = otherNode.spawnPoints.Find(entry => entry.Name == transition.SpawnPoint);

        if (entrance == null) {
          Debug.LogWarning($"Couldn't find spawn '{transition.SpawnPoint}' in scene '{otherNode.NodeName}'");
        } else {
          foreach (var port in otherNode.Outputs) {
            Debug.Log(port.fieldName);

          }
          this.ConnectTo(otherNode, t.Name, entrance.Name);
        }
      });

      doors.ForEach(d => {
        TransitionDoor door = SceneUtils.Find<TransitionDoor>(scene, d.Name);
        SceneNode otherNode = (SceneNode) graph.nodes.Find(node => (node as SceneNode).NodeName == (door.Scene.SceneName + ".unity"));
        if (otherNode == null) {
          Debug.LogWarning($"Couldn't find scene {door.Scene.SceneName}");
          return;
        }
        
        var entrance = otherNode.spawnPoints.Find(entry => entry.Name == door.SpawnName);

        if (entrance == null) {
          Debug.LogWarning($"Couldn't find spawn '{door.SpawnName}' in scene '{otherNode.NodeName}'");
        } else {
          this.ConnectTo(otherNode, d.Name, entrance.Name);
        }
      });
    }

#endif

    public void AddDoor(TransitionDoor door) {
      SceneComponentRef entry = new SceneComponentRef();
      entry.GUID = door.GetComponent<GuidComponent>().GetGuid();
      entry.Name = door.name;
      if (doors.IndexOf(entry) < 0) {
        doors.Add(entry);
        AddDynamicOutput(typeof(Exit), fieldName: entry.Name);
      }
    }

    public void AddTransition(Transition transition) {
      SceneComponentRef entry = new SceneComponentRef();
      entry.GUID = transition.GetComponent<GuidComponent>().GetGuid();
      entry.Name = transition.name;
      if (transitions.IndexOf(entry) < 0) {
        transitions.Add(entry);
        AddDynamicOutput(typeof(Exit), fieldName: entry.Name);
      }
    }

    public void AddSpawnPoint(SpawnPoint spawnPoint) {
      SceneComponentRef entry = new SceneComponentRef();
      entry.GUID = spawnPoint.GetComponent<GuidComponent>().GetGuid();
      entry.Name = spawnPoint.name;
      if (spawnPoints.IndexOf(entry) < 0) {
        spawnPoints.Add(entry);
        AddDynamicInput(typeof(Entrance), fieldName: entry.Name);
      }
    }

    public void Clear() {
      transitions.ForEach(t => RemoveDynamicPort(t.Name));
      doors.ForEach(d => RemoveDynamicPort(d.Name));
      spawnPoints.ForEach(s => RemoveDynamicPort(s.Name));

      transitions.Clear();
      doors.Clear();
      spawnPoints.Clear();
    }
  }
}