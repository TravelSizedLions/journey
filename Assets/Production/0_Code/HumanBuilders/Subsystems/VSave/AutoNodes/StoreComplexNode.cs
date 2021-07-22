using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;


using UnityEditor;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  /// <summary>
  /// A node for storing complex game objects.
  /// </summary>
  [NodeWidth(360)]
  [NodeTint(NodeColors.DYNAMIC_COLOR)]
  [CreateNodeMenu("")]
  [Obsolete("Untested and already deprecated, do not use.")]
  public class StoreComplexNode : AutoNode {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
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
    #endregion

   #region Auto Node API
    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    
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
    #endregion
  }
}