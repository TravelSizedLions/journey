using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A piece of collectible currency with a specific value.
  /// </summary>
  public abstract class Currency : Collectible {

    #region Variables
    #region Basic Settings
    [Header("Basic Settings", order=0)]
    [Space(5, order=1)]

    /// <summary>
    /// The name of the currency system (i.e. "Cash," "Diamonds," "Potataes").
    /// </summary>
    [Tooltip("The name of the currency system (i.e. \"Cash,\" \"Diamonds,\" \"Potataes\").")]
    [SerializeField]
    protected string currencyName;


    /// <summary>
    /// How many "points" this piece of currency is worth
    /// </summary>
    [Tooltip("How many points this piece of currency is worth.")]
    public float Value = 1;

    [Space(10, order=2)]
    #endregion

    #region Sound Settings
    [Header("Sound Settings", order=3)]
    [Space(5, order=4)]

    /// <summary>
    /// Whether or not to play sounds when the player collects this piece of currency.
    /// </summary>
    [Tooltip("Whether or not to play sounds when the player collects this piece of currency.")]
    [SerializeField]
    protected bool playSounds = true;


    /// <summary>
    /// How long to wait before playing the pick up sound.
    /// </summary>
    [Tooltip("How long to wait before playing the pick up sound.")]
    [SerializeField]
    protected float soundDelay = 0.0f;

    #endregion
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    protected override void Awake() {
      base.Awake();
    }

    #endregion

    #region Collectible API
    //-------------------------------------------------------------------------
    // Collectible API
    //-------------------------------------------------------------------------


    /// <summary>
    /// Fires when this piece of currency is collected by the player.
    /// </summary>
    public override void OnCollected() {
      base.OnCollected();
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Get the name of the currency.
    /// </summary>
    public string GetName() {
      return currencyName;
    }

    /// <summary>
    /// Get the value of this piece of currency.
    /// </summary>
    public float GetValue() {
      return Value;
    }


    /// <summary>
    /// Enable the pickup sound effect on this piece of currency.
    /// </summary>
    public void EnableSounds() {
      playSounds = true;
    }


    /// <summary>
    /// Disable the pickup sound effect on this piece of currency.
    /// </summary>
    public void DisableSounds() {
      playSounds = true;
    }

    /// <summary>
    /// Whether or not the pickup sound effect is enabled for this piece of currency.
    /// </summary>
    public bool IsSoundEnabled() {
      return playSounds;
    }

    /// <summary>
    /// How long to wait before playing the pick up sound.
    /// </summary>
    public float GetSoundDelay() {
      return soundDelay;
    }

    #endregion

  }

}