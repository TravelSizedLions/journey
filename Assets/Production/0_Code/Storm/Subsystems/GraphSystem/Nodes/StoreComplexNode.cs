using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Storm.Subsystems.Save;
using UnityEditor;
using UnityEngine;
using XNode;

namespace Storm.Subsystems.Graph {
  /// <summary>
  /// A dialog node for setting a value to 
  /// </summary>
  [NodeWidth(360)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("Dialog/Data/Store Complex Node")]
  public class StoreComplexNode : AutoNode {

    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType=ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// Where to store this value. This is usually the name of the level.
    /// </summary>
    [Tooltip("Where to store this value. This is usually the name of the level.")]
    public string Folder;

    /// <summary>
    /// The object to store.
    /// </summary>
    [Tooltip("The object to store. This game object must contain one or more components that inherits from IStorable.")]
    [ValidateInput("IsStorable")]
    public GameObject Storable;
    

    [Space(8, order=1)]

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Output(connectionType=ConnectionType.Override)]
    public EmptyConnection Output;

    /// <summary>
    /// Get the value of a port.
    /// </summary>
    /// <param name="port">The input/output port.</param>
    /// <returns>The value for the port.</returns>
    public override object GetValue(NodePort port) {
      return null;
    }

    
    public override void Handle(GraphEngine graphEngine) {
      Component[] comps = Storable.GetComponents(typeof(Component));
      foreach (Component comp in comps) {
        IStorable storable = comp as IStorable;
        if (storable != null) {
          storable.Store();
        }
      }
    }

    private bool IsStorable(GameObject obj) {
      if (obj == null) {
        return true;
      }

      Component[] comps = obj.GetComponents(typeof(Component));
      foreach (Component c in comps) {
        IStorable storable = c as IStorable;
        if (storable != null) {
          return true;
        }
      }

      return false;
    }

  }
}