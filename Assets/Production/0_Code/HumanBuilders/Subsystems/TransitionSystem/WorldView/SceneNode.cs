using System.Linq;
using HumanBuilders.Graphing;
using UnityEngine;
using UnityEngine.SceneManagement;
using XNode;
using Sirenix.OdinInspector;
using HumanBuilders.Attributes;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace TSL.Subsystems.WorldView {
  [NodeWidth(600)]
  public class SceneNode : AutoNode {
    public bool CallbacksEnabled => ((WorldViewGraph)graph).CallbacksEnabled;

    private SceneData data;

    public string Path => data.Path;


    [Button("Open", ButtonSizes.Medium), GUIColor(.8f, .5f, 1, .5f)]
    public void OpenScene() {
      if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
        EditorSceneManager.OpenScene(Path, OpenSceneMode.Single);
      }
    }

    public void Construct(SceneData data) {
      this.data = data;
      name = data.Name;

      data.Spawns.ForEach(info => Add(info));
      data.Transitions.ForEach(info => Add(info));
    }


    public bool Sync() {
      if (data.SyncWithScene()) {
        SyncOutputs();
        SyncInputs();
        EditorUtility.SetDirty(this);
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
        if (!data.ContainsSpawn(input.fieldName)) {
          RemoveDynamicPort(input.fieldName);
        }
      });
    }

    public void AddNewInputs() {
      data.Spawns.ForEach(info => {
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
        if (!data.ContainsTransition(output.fieldName)) {
          RemoveDynamicPort(output.fieldName);
        }
      });
    }

    public void AddNewOutputs() {
      data.Transitions.ForEach(info => {
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
      data.Transitions.ForEach(info => {
        SceneNode other = ((WorldViewGraph) graph) [info.TargetSceneName];
        if (other != null) {
          ConnectTo(other, info.Name, info.SpawnName);
        } else {
          Debug.Log($"Could not find node {info.TargetSceneName}");
        }
      });
      EnableCallbacks();
    }

    /// <summary> Called after a connection between two <see cref="NodePort"/>s is created </summary>
    /// <param name="from">Output</param> <param name="to">Input</param>
    public override void OnCreateConnection(NodePort from, NodePort to) {
      if (CallbacksEnabled && from.node == this) {
        Debug.Log($"create connection: {from.node.name} {from.fieldName} to {to.node.name} {to.fieldName}");
        data.OnCreateConnection(from, to);
      }
    }

    /// <summary> Called after a connection is removed from this port </summary>
    /// <param name="port">Output or Input</param>
    public override void OnRemoveConnection(NodePort port) {
      if (CallbacksEnabled && port.IsOutput) {
        Debug.Log($"remove connection: {port.node.name} {port.fieldName}");
        data.OnRemoveConnection(port);
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