#if UNITY_EDITOR
using System.Linq;
using HumanBuilders.Graphing;
using UnityEngine;
using UnityEngine.SceneManagement;
using XNode;
using Sirenix.OdinInspector;
using HumanBuilders.Attributes;
using UnityEditor.SceneManagement;

using UnityEditor;


namespace TSL.Subsystems.WorldView {
  [NodeWidth(600)]
  public class SceneNode : AutoNode {
    public bool CallbacksEnabled => ((WorldViewGraph)graph).CallbacksEnabled;

    [ReadOnly]
    public SceneData Data;

    public string Path => Data.Path;


    [Button("Open", ButtonSizes.Medium), GUIColor(.8f, .5f, 1, .5f)]
    public void OpenScene() {
      if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
        EditorSceneManager.OpenScene(Path, OpenSceneMode.Single);
      }
    }

    public void Construct(SceneData data) {
      this.Data = data;
      name = data.Name;

      data.Spawns.ForEach(info => Add(info));
      data.Transitions.ForEach(info => Add(info));
    }


    public bool Sync() {
      Undo.RecordObject(this, $"Sync node with scene {this.name}");
      if (Data.SyncWithScene()) {
        SyncOutputs();
        SyncInputs();
        return true;
      }
      return false;
    }

    public void SyncInputs() {
      RemoveUnusedInputs();
      AddNewInputs();
    }

    public void RemoveUnusedInputs() {
      Inputs.ToList().ForEach(input => {
        if (!Data.ContainsSpawn(input.fieldName)) {
          RemoveDynamicPort(input.fieldName);
        }
      });
    }

    public void AddNewInputs() {
      Data.Spawns.ForEach(info => {
        if (!Contains(info)) {
          Add(info);
        }
      });
    }

    public void SyncOutputs() {
      RemoveUnusedOutputs();
      AddNewOutputs();
      RebuildTransitions();
    }

    public void RemoveUnusedOutputs() {
      Outputs.ToList().ForEach(output => {
        if (!Data.ContainsTransition(output.fieldName)) {
          RemoveDynamicPort(output.fieldName);
        }
      });
    }

    public void AddNewOutputs() {
      Data.Transitions.ForEach(info => {
        if (!Contains(info)) {
          Add(info);
        }
      });
    }

    public bool Contains(TransitionData data) => GetOutputPort(data.Name) != null;

    public bool Contains(SpawnData data) => GetInputPort(data.Name) != null;

    public void Add(TransitionData data) => AddDynamicOutput(typeof(Exit), connectionType: ConnectionType.Override, fieldName : data.Name);

    public void Add(SpawnData data) => AddDynamicInput(typeof(Entrance), fieldName : data.Name);

    public void RebuildTransitions() {
      DisableCallbacks();
      Debug.Log($"building transitions for {name}");
      Outputs.ToList().ForEach(output => output.ClearConnections());
      Data.Transitions.ForEach(info => {
        SceneNode other = ((WorldViewGraph) graph) [info.TargetSceneName];
        if (other != null) {
          ConnectTo(other, info.Name, info.SpawnName);
        } else {
          Debug.Log($"Could not find node {info.TargetSceneName}");
        }
      });
      EnableCallbacks();
    }

    public override void OnCreateConnection(NodePort from, NodePort to) {
      if (CallbacksEnabled && from.node == this) {
        Undo.RecordObjects(new Object[] {from.node, to.node}, $"create connection: {from.node.name} {from.fieldName} to {to.node.name} {to.fieldName}");
        Data.OnCreateConnection(from, to);
        AssetDatabase.SaveAssets();
      }
    }

    public override void OnRemoveConnection(NodePort port) {
      Undo.RecordObject(this, $"remove connection: {port.node.name} {port.fieldName}");
      if (CallbacksEnabled && port.IsOutput) {
        Data.OnRemoveConnection(port);
        AssetDatabase.SaveAssets();
      }
    }

    public void DisableCallbacks() {
      ((WorldViewGraph)graph).DisableCallbacks();
    }

    public void EnableCallbacks() {
      ((WorldViewGraph)graph).EnableCallbacks();
    }
  }
}

#endif
