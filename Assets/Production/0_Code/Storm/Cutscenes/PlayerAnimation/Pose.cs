using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Playables;
using System;
using System.Reflection;
using Storm.Characters.Player;
using UnityEngine.Animations;
using Storm.Subsystems.FSM;
using Storm.Characters;

namespace Storm.Cutscenes {
  /// <summary>
  /// An abstract base class representing a track clip for posing the player.
  /// </summary>
  public abstract class Pose : PlayableAsset {

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

    #endregion

    #region Playable API

    /// <summary>
    /// Create a pose asset.
    /// </summary>
    /// <param name="graph">The playable graph this pose will belong to.</param>
    /// <param name="owner">IDK my BFF jill.</param>
    /// <returns>A script playable for the pose</returns>
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner) {
      // Use hook method to get the right type of pose info
      ScriptPlayable<PoseTemplate> playable = CreateScriptPlayable(graph);
      PoseTemplate p = playable.GetBehaviour();

      if (State != null) {
        p.State = Type.GetType(State);
      }

      return playable;
    }

    #endregion

    #region Abstract Methods
    //-------------------------------------------------------------------------
    // Abstract Methods
    //-------------------------------------------------------------------------

    /// <summary>
    /// (Hook Method) Create the script playable for the clip's pose type.
    /// </summary>
    /// <seealso cref="AbsolutePose.CreateScriptPlayable"/>
    /// <seealso cref="RelativePose.CreateScriptPlayable"/>
    public abstract ScriptPlayable<PoseTemplate> CreateScriptPlayable(PlayableGraph graph) ;

    #endregion

    #region Odin Inspector Stuff
    //-------------------------------------------------------------------------
    // Odin Inspector
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Get a list of the player's types.
    /// </summary>
    private ValueDropdownList<string> GetStateTypes() {
      ValueDropdownList<string> types = new ValueDropdownList<string>();

      foreach (Type t in Pose.GetSubtypesOfTypeInAssembly("Storm", typeof(PlayerState))) {
        string typeName = t.ToString();

        string[] subs = typeName.Split('.');
        string simpleName = subs[subs.Length-1];
        string letter = (""+simpleName[0]).ToUpper();

        string folder = letter + "/" + simpleName;

        types.Add(new ValueDropdownItem<string>(folder, typeName));
      }

      return types;
    }

    /// <summary>
    /// Within a code assembly, searches for all subtypes of the given type.
    /// </summary>
    /// <param name="assemblyName">The name of the C# assembly.</param>
    /// <param name="t">The type to search for.</param>
    /// <returns>The list of types in the assebly that are a subtype of t.</returns>
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
  #endregion
}