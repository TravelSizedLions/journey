using UnityEngine;

using UnityEngine.SceneManagement;

namespace HumanBuilders {

  /// <summary>
  /// A component for animated game objects that need to save their animation
  /// state between scenes.
  /// </summary>
  [RequireComponent(typeof(Animator))]
  [RequireComponent(typeof(GuidComponent))]
  public class SaveAnimator : MonoBehaviour, IStorable {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The animator for this component.
    /// </summary>
    private Animator anim;

    /// <summary>
    /// The animator controller's parameters.
    /// </summary>
    private AnimatorControllerParameter[] parameters;

    /// <summary>
    /// The animator controller's current state. This updates once per frame
    /// because, for some reason, there is no current state data during OnDestroy.
    /// </summary>
    private AnimatorStateInfo curState;

    /// <summary>
    /// A reference to this guid.
    /// </summary>
    private GuidComponent guid;

    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      guid = GetComponent<GuidComponent>();
      if (guid == null) {
        Debug.LogWarning("Game object \"" + name + "\" is missing a GuidComponent! Add one in the inspector.");
      }

      anim = GetComponent<Animator>();
      if (anim == null) {
        Debug.LogWarning("Game object \"" + name + "\" is missing an Animator Component! Add one in the inspector.");
      }

      parameters = anim.parameters;

      // Load the previous state.
      Retrieve();
    }

    private void Update() {
      AnimatorStateInfo cur = anim.GetCurrentAnimatorStateInfo(0);
      if (cur.fullPathHash != 0) {
        curState = cur;
      }
    }


    /// <summary>
    /// Stores the animator's state when the scene changes.
    /// </summary>
    /// <param name="unloadedScene">The scene that was unloaded.</param>
    private void OnDestroy() {
      Store();
    }

    #endregion


    #region Storable API
    //-------------------------------------------------------------------------
    // Storable API
    //-------------------------------------------------------------------------

    /// <summary>
    /// Retrieve state information.
    /// </summary>
    public void Retrieve() {
      foreach (AnimatorControllerParameter param in parameters) {
        RetrieveParameter(param);
      }

      RetrieveState();
    }

    /// <summary>
    /// Load the previous state of an animation parameter
    /// </summary>
    /// <param name="param">The animation parameter to load.</param>
    private void RetrieveParameter(AnimatorControllerParameter param) {
      string key = guid.ToString()+"_"+param.name;
      string folder = StaticFolders.ANIMATION;
      
      // Set the value based on the parameter type.
      switch (param.type) {
        case AnimatorControllerParameterType.Int: {
          key += Keys.ANIM_INT; 
          if (VSave.Get(folder, key, out int value)) {
            anim.SetInteger(param.name, value);
          }
          break;
        }
        case AnimatorControllerParameterType.Bool: {
          key += Keys.ANIM_BOOL;
          if (VSave.Get(folder, key, out bool value)) {
            anim.SetBool(param.name, value);
          }
          break;
        }
        case AnimatorControllerParameterType.Float: {
          key += Keys.ANIM_FLOAT; 
          if (VSave.Get(folder, key, out float value)) {
            anim.SetFloat(param.name, value);
          }
          break;
        }
        default: return; // Triggers fire one off. They don't need to be stored.
      }
    }

    /// <summary>
    /// Load the previous animator state.
    /// </summary>
    private void RetrieveState() {
      string key = guid.ToString()+Keys.ANIM_STATE;
      string folder = StaticFolders.ANIMATION;

      var state = anim.GetCurrentAnimatorStateInfo(0);

      if (VSave.Get(folder, key, out int stateHashCode)) {
        anim.Play(stateHashCode);
      }
    }

    /// <summary>
    /// Store information about the animator's state & parameters.
    /// </summary>
    public void Store() {
      foreach (AnimatorControllerParameter param in parameters) {
        StoreParameter(param);
      }

      StoreState();
    }

    /// <summary>
    /// Store information about a single animation parameter.
    /// </summary>
    /// <param name="param">The animation parameter to store.</param>
    private void StoreParameter(AnimatorControllerParameter param) {
      string key = guid.ToString()+"_"+param.name;
      dynamic value = null;
      
      // Set the value based on the parameter type.
      switch (param.type) {
        case AnimatorControllerParameterType.Int: {
          key += Keys.ANIM_INT; 
          value = anim.GetInteger(param.name);
          break;
        }
        case AnimatorControllerParameterType.Bool: {
          key += Keys.ANIM_BOOL;
          value = anim.GetBool(param.name);
          break;
        }
        case AnimatorControllerParameterType.Float: {
          key += Keys.ANIM_FLOAT; 
          value = anim.GetFloat(param.name);
          break;
        }
        default: return; // Triggers fire one off. They don't need to be stored.
      }

      if (value != null) {
        VSave.Set(StaticFolders.ANIMATION, key, value);
      } 
    }

    /// <summary>
    /// Store the animator's state.
    /// </summary>
    private void StoreState() {
      int hash = curState.fullPathHash;

      string key = guid.ToString()+Keys.ANIM_STATE;
      string folder = StaticFolders.ANIMATION;

      VSave.Set(folder, key, hash);
    }

    #endregion
  }
}