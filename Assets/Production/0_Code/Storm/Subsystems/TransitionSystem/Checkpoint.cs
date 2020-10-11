using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Storm.Attributes;

namespace Storm.Subsystems.Transitions {

  /// <summary>
  /// 
  /// </summary>
  public class Checkpoint : MonoBehaviour {

    /// <summary>
    /// The spawn point that the player will respawn at after hitting this checkpoint.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    [Tooltip("The spawn point that the player will respawn at after hitting this checkpoint.")]
    private SpawnPoint spawn;

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Start() {
      if (spawn == null) {
        spawn = GetComponentInChildren<SpawnPoint>();
      }
    }

    private void OnTriggerEnter2D(Collider2D col) {
      if (col.CompareTag("Player")) {
        TransitionManager.SetCurrentSpawn(spawn.name);
      }
    }
    #endregion
  }

}