using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    private SceneData data;

    public string Path => data.Path;

    public void Construct(SceneData data) {
      this.data = data;
      name = data.Name;

      data.Spawns.ForEach(info => Add(info));
      data.Transitions.ForEach(info => Add(info));
    }

    public bool Sync(Scene scene) {
      if (data.Sync(scene)) {
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

    public void Add(TransitionData data) => AddDynamicOutput(typeof(Exit), fieldName : data.Name);

    public void Add(SpawnData data) => AddDynamicInput(typeof(Entrance), fieldName : data.Name);
  }
}