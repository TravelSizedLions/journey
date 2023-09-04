﻿using System;
using UnityEngine;
using UnityEngine.GUID;

namespace HumanBuilders {

  /// <summary>
  /// A location the player will respawn at if they die or enter the level from another scene.
  /// </summary>
  [RequireComponent(typeof(GuidComponent))]
  public class SpawnPoint : MonoBehaviour {

    #region Variables
    /// <summary>
    /// Whether or not the player should be facing right when they spawn.
    /// </summary>
    [Tooltip("Whether or not the player should be facing right when they spawn.")]
    public bool SpawnFacingRight;

    public Guid ID => GetComponent<GuidComponent>().GetGuid();

    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      TransitionManager.RegisterSpawn(this.name, transform.position, SpawnFacingRight);
    }
    #endregion
  }
}