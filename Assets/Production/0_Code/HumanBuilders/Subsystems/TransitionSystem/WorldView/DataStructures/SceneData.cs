using System;
using System.Collections.Generic;
using System.Linq;
using HumanBuilders;
using TSL.Editor.SceneUtilities;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace TSL.Subsystems.WorldView {
  [Serializable]
  public class SceneData {
    public string Path => AssetDatabase.GUIDToAssetPath(guid);
    public string Name => name;
    private readonly string name;
    private readonly GUID guid;
    private List<SpawnData> spawns = new List<SpawnData>();
    private List<TransitionData> transitions = new List<TransitionData>();

    public List<SpawnData> Spawns => spawns;
    public List<TransitionData> Transitions => transitions;

    public SceneData(string path) {
      this.guid = AssetDatabase.GUIDFromAssetPath(path);
      List<string> openScenes = SceneUtils.GetOpenScenePaths();
      Scene scene = openScenes.Contains(path) ? EditorSceneManager.GetSceneByPath(path) : EditorSceneManager.OpenScene(path, OpenSceneMode.Additive);
      SceneUtils.FindAll<TransitionDoor>(scene).ForEach(t => Add(t));
      SceneUtils.FindAll<Transition>(scene).ForEach(t => Add(t));
      SceneUtils.FindAll<SpawnPoint>(scene).ForEach(s => Add(s));

      if (!openScenes.Contains(path)) {
        EditorSceneManager.CloseScene(scene, true);
      }
      
      name = path.Split('/') [path.Split('/').Length - 1].Split('.') [0];
    }

    public SceneData(Scene scene) {
      name = scene.name;
      this.guid = AssetDatabase.GUIDFromAssetPath(scene.path);
      SceneUtils.FindAll<TransitionDoor>(scene).ForEach(t => Add(t));
      SceneUtils.FindAll<Transition>(scene).ForEach(t => Add(t));
      SceneUtils.FindAll<SpawnPoint>(scene).ForEach(s => Add(s));
    }

    public void Add(Transition transition) {
      if (!Contains(transition)) {
        transitions.Add(new TransitionData(transition));
      }
    }

    public void Add(TransitionDoor transition) {
      if (!Contains(transition)) {
        transitions.Add(new TransitionData(transition));
      }
    }

    public void Add(SpawnPoint spawn) {
      if (!Contains(spawn)) {
        spawns.Add(new SpawnData(spawn));
      }
    }

    public bool ContainsTransition(string name) {
      return transitions.Find(info => info.Name == name) != null;
    }

    public bool Contains(Transition transition) {
      return transitions.FindIndex(info => info.ID == transition.ID) >= 0;
    }

    public bool Contains(TransitionDoor transition) {
      return transitions.FindIndex(info => info.ID == transition.ID) >= 0;
    }

    public bool Contains(TransitionData transition) {
      return transitions.IndexOf(transition) >= 0;
    }

    public bool Contains(SpawnPoint spawn) {
      return spawns.FindIndex(info => info.ID == spawn.ID) >= 0;
    }

    public bool Contains(SpawnData spawn) {
      return spawns.IndexOf(spawn) >= 0;
    }

    public bool ContainsSpawn(string name) {
      return spawns.Find(info => info.Name == name) != null;
    }

    public TransitionData Get(Transition transition) {
      return transitions.Find(info => info.ID == transition.ID);
    }

    public TransitionData Get(TransitionDoor transition) {
      return transitions.Find(info => info.ID == transition.ID);
    }

    public SpawnData Get(SpawnPoint spawn) {
      return spawns.Find(info => info.ID == spawn.ID);
    }

    public SpawnData GetSpawn(string name) {
      return spawns.Find(info => info.Name == name);
    }

    public void Remove(Transition transition) {
      if (Contains(transition)) {
        transitions.Remove(Get(transition));
      }
    }

    public void Remove(TransitionData transition) {
      transitions.Remove(transition);
    }

    public void Remove(TransitionDoor transition) {
      if (Contains(transition)) {
        transitions.Remove(Get(transition));
      }
    }

    public void Remove(SpawnPoint spawn) {
      if (Contains(spawn)) {
        spawns.Remove(Get(spawn));
      }
    }

    public void Remove(SpawnData spawn) {
      spawns.Remove(spawn);
    }

    public bool SyncWithScene() {
      List<string> openScenes = SceneUtils.GetOpenScenePaths();
      Scene scene = openScenes.Contains(Path) ? EditorSceneManager.GetSceneByPath(Path) : EditorSceneManager.OpenScene(Path, OpenSceneMode.Additive);
      bool changed = Sync(scene);
      if (!openScenes.Contains(Path)) {
        EditorSceneManager.CloseScene(scene, true);
      }
      return changed;
    }

    /// <summary>
    /// Sync this data with what's in the scene.
    /// </summary>
    /// <param name="scene">The scene to sync with</param>
    /// <returns>true if there were changes. False otherwise.</returns>
    private bool Sync(Scene scene) {
      SceneData updated = new SceneData(scene);
      if (updated == this) {
        return false;
      }

      spawns = updated.spawns;
      transitions = updated.transitions;
      return true;
    }

    public bool Update(Transition transition) {
      TransitionData data = Get(transition);
      if (data != null) {
        Remove(data);
        Add(transition);
        return true;
      }

      return false;
    }

    public bool Update(TransitionDoor transition) {
      TransitionData data = Get(transition);
      if (data != null) {
        Remove(data);
        Add(transition);
        return true;
      }

      return false;
    }

    public bool Update(SpawnPoint spawn) {
      SpawnData data = Get(spawn);
      if (data != null) {
        Remove(data);
        Add(spawn);
        return true;
      }

      return false;
    }

    public override bool Equals(object obj) {
      if (obj.GetType() != this.GetType()) {
        return false;
      }

      SceneData other = (SceneData) obj;
      if (spawns.Count != other.spawns.Count) {
        return false;
      }

      if (transitions.Count != other.transitions.Count) {
        return false;
      }

      if (spawns.Any(s => !other.Contains(s))) {
        return false;
      }

      if (transitions.Any(t => !other.Contains(t))) {
        return false;
      }

      return true;
    }

    public override int GetHashCode() {
      return base.GetHashCode();
    }
  }
}