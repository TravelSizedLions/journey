using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;

using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// This node has the player automatically walk towards a target position.
  /// </summary>
  [NodeTint(NodeColors.ANIMATION_COLOR)]
  [NodeWidth(NodeWidths.SHORT)]
  [CreateNodeMenu("Player/Set Animation")]
  public class SetPlayerAnimationNode : AutoNode {

    //-------------------------------------------------------------------------
    // Node Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// Input connection from the previous node(s).
    /// </summary>
    [Input(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Input;

    [Space(8, order=0)]

    /// <summary>
    /// The name of the parameter to set.
    /// </summary>
    [Tooltip("The name of the parameter to set.")]
    [SerializeField]
    [ValueDropdown(nameof(GetStateTypes))]
    public string State;

    /// <summary>
    /// Output connection for the next node.
    /// </summary>
    [Space(8, order=2)]
    [Output(connectionType = ConnectionType.Override)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Auto Node API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      if (State != null) {
        Type t = Type.GetType(State);

        if (t != null) {
          StateDriver driver = StateDriver.For(State);
          if (!driver.IsInState(GameManager.Player.FSM)) {
            driver.ForceStateChangeOn(player.FSM);
          }
        }
      }
    }

    //-------------------------------------------------------------------------
    // Odin Inspector Stuff
    //-------------------------------------------------------------------------

    /// <summary>
    /// Get a list of the player's types.
    /// </summary>
    private ValueDropdownList<string> GetStateTypes() {
      ValueDropdownList<string> types = new ValueDropdownList<string>();
      
      #if UNITY_EDITOR
      foreach (Type t in EditorUtils.GetSubtypesOfTypeInAssembly("HumanBuilders", typeof(PlayerState))) {
        string typeName = t.ToString();

        string[] subs = typeName.Split('.');
        string simpleName = subs[subs.Length - 1];
        string letter = ("" + simpleName[0]).ToUpper();

        string folder = letter + "/" + simpleName;

        types.Add(new ValueDropdownItem<string>(folder, typeName));
      }
      #endif

      return types;
    }
  }

}