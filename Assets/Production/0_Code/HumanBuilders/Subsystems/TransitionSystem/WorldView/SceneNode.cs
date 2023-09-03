using System;
using System.Collections;
using System.Collections.Generic;
using HumanBuilders;
using HumanBuilders.Attributes;
using HumanBuilders.Graphing;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.GUID;
using UnityEngine.SceneManagement;
using XNode;

#if UNITY_EDITOR
using TSL.Editor.SceneUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
#endif


namespace TSL.Subsystems.WorldView {
  [NodeWidth(600)]
  public class SceneNode : AutoNode {
    public SceneField Scene { get => sceneField; }
    private SceneField sceneField;

    public string Path { get => AssetDatabase.GUIDToAssetPath(guid); }

    public GUID AssetGUID { get => guid; }
    private GUID guid;

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

    /// <summary>
    /// Called after a connection between two <see cref="NodePort"/>s is created
    /// </summary>
    /// <param name="from">Output</param>
    /// <param name="to">Input</param>
    public override void OnCreateConnection(NodePort from, NodePort to) {
      if (from.node == this) {
        // WorldViewSynchronizer.Disable();
        List<Scene> openScenes = SceneUtils.GetOpenScenes();

        // gather transition info
        string transitionName = from.fieldName;
        SceneNode toNode = (to.node as SceneNode);
        SceneAsset toSceneAsset = (SceneAsset) toNode.sceneField.SceneAsset;
        string toSpawnPoint = to.fieldName;

        // open the scene to edit
        string fromPath = AssetDatabase.GUIDToAssetPath(guid);
        Scene fromScene = EditorSceneManager.OpenScene(fromPath, OpenSceneMode.Additive);

        // update the transition
        SetTransition(fromScene, transitionName, toSceneAsset, toSpawnPoint);

        if (!openScenes.Contains(fromScene)) {
          SceneUtils.SaveAndClose(fromScene);
        } else {
          EditorSceneManager.SaveScene(fromScene);
        }
        // WorldViewSynchronizer.Enable();
      }
    }

    /// <summary>
    /// Called after a connection is removed from this port
    /// </summary>
    /// <param name="port">Output or Input</param>
    public override void OnRemoveConnection(NodePort port) {
      if (port.IsOutput) {
        // WorldViewSynchronizer.Disable();
        List<Scene> openScenes = SceneUtils.GetOpenScenes();

        // gather transition info
        string transitionName = port.fieldName;

        // open scene to edit
        string path = AssetDatabase.GUIDToAssetPath(guid);
        Scene targetScene = EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);

        // update the transition
        SetTransition(targetScene, transitionName, null, "");

        if (!openScenes.Contains(targetScene)) {
          SceneUtils.SaveAndClose(targetScene);
        } else {
          EditorSceneManager.SaveScene(targetScene);
        }
        // WorldViewSynchronizer.Enable();
      }
    }

    private void SetTransition(Scene fromScene, string transitionName, SceneAsset toScene, string toSpawn) {
      Transition transition = SceneUtils.Find<Transition>(fromScene, transitionName);
      UnityEngine.Object oldScene = null;
      string oldSpawn = "";
      if (transition != null) {
        try {
          oldScene = transition.Scene.SceneAsset;
          oldSpawn = transition.SpawnPoint;
          transition.Scene.SceneAsset = toScene;
          transition.SpawnPoint = toSpawn;
          return;
        } catch (Exception e) {
          transition.Scene.SceneAsset = oldScene;
          transition.SpawnPoint = oldSpawn;
          Debug.LogError(e);
        }
      }

      TransitionDoor door = SceneUtils.Find<TransitionDoor>(fromScene, transitionName);
      if (door != null) {
        try {
          oldScene = door.Scene.SceneAsset;
          oldSpawn = door.SpawnName;
          door.Scene.SceneAsset = toScene;
          door.SpawnName = toSpawn;
          return;
        } catch (Exception e) {
          door.Scene.SceneAsset = oldScene;
          door.SpawnName = oldSpawn;
          Debug.LogError(e);
        }
      }

      Debug.LogWarning($"Could not find transition with name {transitionName} in scene {fromScene.name}");
    }


#if UNITY_EDITOR
    public void Construct(string scenePath) {
      sceneField = new SceneField();
      sceneField.SceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
      guid = AssetDatabase.GUIDFromAssetPath(scenePath);

      // "Path/To/my-scene.unity" -> "my-scene"
      name = scenePath.Split('/') [scenePath.Split('/').Length - 1].Split('.') [0];
    }

    public void Sync(Scene scene) {
      EditorUtility.SetDirty(this);
      ClearExits();
      SceneUtils.FindAll<TransitionDoor>(scene).ForEach(d => AddDoor(d));
      SceneUtils.FindAll<Transition>(scene).ForEach(t => AddTransition(t));
      SceneUtils.FindAll<SpawnPoint>(scene).ForEach(s => AddSpawnPoint(s));
      Connect();
    }

    [ContextMenu("Find Transitions")]
    public void Connect() {
      string path = AssetDatabase.GUIDToAssetPath(guid);
      Scene scene = EditorSceneManager.GetSceneByPath(path);
      if (!scene.IsValid() || !scene.isLoaded) {
        Debug.LogWarning($"Scene not valid: {path}");
        return;
      }

      transitions.ForEach(t => {
        Debug.Log($"NAME: {t.Name}");
        Transition transition = SceneUtils.Find<Transition>(scene, t.Name);

        SceneNode otherNode = (SceneNode) graph.nodes.Find(node => (node as SceneNode).NodeName == transition.Scene.SceneName);
        if (otherNode == null) {
          Debug.LogWarning($"Couldn't find scene '{transition.Scene.SceneName}'");
          return;
        }

        var entrance = otherNode.spawnPoints.Find(entry => entry.Name == transition.SpawnPoint);

        if (entrance == null) {
          Debug.LogWarning($"Couldn't find spawn '{transition.SpawnPoint}' in scene '{otherNode.NodeName}'");
        } else {
          this.ConnectTo(otherNode, t.Name, entrance.Name);
          EditorUtility.SetDirty(otherNode);
        }
      });

      doors.ForEach(d => {
        TransitionDoor door = SceneUtils.Find<TransitionDoor>(scene, d.Name);
        SceneNode otherNode = (SceneNode) graph.nodes.Find(node => (node as SceneNode).NodeName == door.Scene.SceneName);
        if (otherNode == null) {
          Debug.LogWarning($"Couldn't find scene '{door.Scene.SceneName}'");
          return;
        }

        var entrance = otherNode.spawnPoints.Find(entry => entry.Name == door.SpawnName);

        if (entrance == null) {
          Debug.LogWarning($"Couldn't find spawn '{door.SpawnName}' in scene '{otherNode.NodeName}'");
        } else {
          this.ConnectTo(otherNode, d.Name, entrance.Name);
          EditorUtility.SetDirty(otherNode);
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
        AddDynamicOutput(typeof(Exit), connectionType : ConnectionType.Override, fieldName : entry.Name);
      } else {
        Debug.LogWarning($"Door already exists '{entry.Name}'");
      }
    }

    public void AddTransition(Transition transition) {
      SceneComponentRef entry = new SceneComponentRef();
      entry.GUID = transition.GetComponent<GuidComponent>().GetGuid();
      entry.Name = transition.name;
      if (transitions.IndexOf(entry) < 0) {
        transitions.Add(entry);
        AddDynamicOutput(typeof(Exit), connectionType : ConnectionType.Override, fieldName : entry.Name);
      } else {
        Debug.LogWarning($"Transition already exists '{entry.Name}'");
      }
    }

    public void AddSpawnPoint(SpawnPoint spawnPoint) {
      SceneComponentRef entry = new SceneComponentRef();
      entry.GUID = spawnPoint.GetComponent<GuidComponent>().GetGuid();
      entry.Name = spawnPoint.name;
      if (spawnPoints.IndexOf(entry) < 0) {
        spawnPoints.Add(entry);
        AddDynamicInput(typeof(Entrance), fieldName : entry.Name);
      } else {
        Debug.LogWarning($"Spawn already exists '{entry.Name}'");
      }
    }

    public void Clear() {
      ClearExits();
      spawnPoints.ForEach(s => RemoveDynamicPort(s.Name));
      spawnPoints.Clear();
    }

    public void ClearExits() {
      transitions.ForEach(t => RemoveDynamicPort(t.Name));
      doors.ForEach(d => RemoveDynamicPort(d.Name));

      transitions.Clear();
      doors.Clear();
    }
  }
}