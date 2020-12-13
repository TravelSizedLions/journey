using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Reflection;
using Storm.Characters.Player;
using UnityEngine.Animations;

namespace Storm.Cutscenes {
  public class PlayerControlAsset : PlayableAsset {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The state that should be updated in the player's Finite State Machine.
    /// </summary>
    [Tooltip("The state that should be updated in the player's Finite State Machine.")]
    [ValueDropdown("GetStateTypes")]
    public string State;

    /// <summary>
    /// The animation that should be playing in the timeline.
    /// </summary>
    [Tooltip("The animation that should be playing in the timeline.")]
    public AnimationClip Clip;

    #endregion

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
      ScriptPlayable<PlayerControl> playable = ScriptPlayable<PlayerControl>.Create(graph);

      PlayerControl behavior = playable.GetBehaviour();
      behavior.State = Type.GetType(State);
      behavior.Clip = AnimationClipPlayable.Create(graph, Clip);

      return playable;
    }


    private ValueDropdownList<string> GetStateTypes() {
      ValueDropdownList<string> types = new ValueDropdownList<string>();

      foreach (Type t in PlayerControlAsset.GetSubtypesOfTypeInAssembly("Storm", typeof(PlayerState))) {
        string typeName = t.ToString();

        string[] subs = typeName.Split('.');
        string simpleName = subs[subs.Length-1];
        string letter = (""+simpleName[0]).ToUpper();

        string folder = letter + "/" + simpleName;

        types.Add(new ValueDropdownItem<string>(folder, typeName));
      }

      return types;
    }

    private static List<Type> GetSubtypesOfTypeInAssembly(string assemblyName, Type t) {
      List<Type> results = new List<Type>();

      foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
        if (assembly.FullName.StartsWith(assemblyName)) {
          foreach (Type type in assembly.GetTypes()) {
            if (type.IsSubclassOf(t))
            results.Add(type);
          }
          break;
        }
      }

      return results;
    }

  }
}