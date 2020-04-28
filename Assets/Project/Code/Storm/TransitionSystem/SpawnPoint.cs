using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.TransitionSystem {

  /// <summary>
  /// A location the player will respawn at if they die or enter the level from another scene.
  /// </summary>
  public class SpawnPoint : MonoBehaviour {

    #region Variables
    /// <summary>
    /// Whether or not the player should be facing right when they spawn.
    /// </summary>
    [Tooltip("Whether or not the player should be facing right when they spawn.")]
    public bool SpawnFacingRight;

    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      GameManager.Instance.transitions.RegisterSpawn(this.name, transform.position, SpawnFacingRight);
    }

    #endregion
  }
}