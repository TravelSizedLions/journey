
using UnityEngine;

namespace HumanBuilders {
  
  /// <summary>
  /// A snapshot of important visual infomation for the player character.
  /// </summary>
  public class PlayerSnapshot {
    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// The player's position (transform.position).
    /// </summary>
    public Vector3 Position { get { return position; } }

    /// <summary>
    /// The player's euler rotation (transform.eulerAngles).
    /// </summary>
    public Vector3 Rotation { get { return rotation; } }

    /// <summary>
    /// The player's local scale (transform.localScale).
    /// </summary>
    public Vector3 Scale { get { return scale; } } 

    /// <summary>
    /// Whether or not the player's root game object is active.
    /// </summary>
    public bool Active { get { return active; } }

    /// <summary>
    /// The player's current sprite (aka "Animation Frame")
    /// </summary>
    public Sprite Sprite { get {return sprite;} }

    /// <summary>
    /// The player's collider bounds.
    /// </summary>
    public Vector2 ColliderSize { get { return colliderSize; } }

    /// <summary>
    /// The player's collider offset.
    /// </summary>
    public Vector2 ColliderOffset { get { return colliderOffset; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// A driver for the state that the player's finite state machine was
    /// previously in. This allows the finite state machine to roll back to
    /// its previous state.
    /// </summary>
    private StateDriver driver;

    /// <summary>
    /// The player's position.
    /// </summary>
    private Vector3 position;

    /// <summary>
    /// The player's local euler rotation.
    /// </summary>
    private Vector3 rotation;

    /// <summary>
    /// The player's local scale.
    /// </summary>
    private Vector3 scale;

    /// <summary>
    /// Whether or not the player is facing left or right.
    /// </summary>
    private Facing facing;

    /// <summary>
    /// Whether or not the root game object for the player is active.
    /// </summary>
    private bool active;

    /// <summary>
    /// The player's current sprite (aka "animation frame").
    /// </summary>
    private Sprite sprite;

    /// <summary>
    /// The player's collider bounds.
    /// </summary>
    private Vector3 colliderSize;

    /// <summary>
    /// The player's collider offset.
    /// </summary>
    private Vector2 colliderOffset;
    #endregion


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="player">The player character.</param>
    public  PlayerSnapshot(PlayerCharacter player) {
      if (player != null) {
        driver = StateDriver.For(player.FSM.CurrentState);
        position = player.transform.position;
        rotation = player.transform.eulerAngles;
        scale = player.transform.localScale;
        facing = player.Facing;
        active = player.gameObject.activeSelf;
        sprite = player.Sprite.sprite;
        colliderSize = player.Collider.size;
        colliderOffset = player.Collider.offset;
      }
    }

    public void Restore(PlayerCharacter player) {
      RestoreTransform(player);
      RestoreCollider(player);
      RestoreFacing(player);
      RestoreSprite(player);
      RestoreActive(player);
      RestoreState(player);
    }

    /// <summary>
    /// Restore the transform the player was in.
    /// </summary>
    /// <param name="player">The player character.</param>
    public void RestoreTransform(PlayerCharacter player) {
      player.transform.position = position;
      player.transform.eulerAngles = rotation;
      player.transform.localScale = scale;
    }

    public void RestoreFacing(PlayerCharacter player) {
      player.SetFacing(facing);
    }

    public void RestoreSprite(PlayerCharacter player) {
      player.Sprite.sprite = sprite;
    }

    public void RestoreActive(PlayerCharacter player) {
      player.gameObject.SetActive(active);
    }

    public void RestoreCollider(PlayerCharacter player) {
      player.Collider.size = colliderSize;
      player.Collider.offset = colliderOffset;
    }

    /// <summary>
    /// Restore the state the player's state machine was in.
    /// </summary>
    /// <param name="player">The player character.</param>
    public void RestoreState(PlayerCharacter player) {
      #if UNITY_EDITOR
      if (Application.isPlaying) {
      #endif

      if (driver != null && !driver.IsInState(player.FSM)) {
        driver.ForceStateChangeOn(player.FSM);
      }

      player.SetFacing(facing);
      player.gameObject.SetActive(active);
      player.Sprite.sprite = sprite;
      
      #if UNITY_EDITOR
      }
      #endif
    }
  }
}