using UnityEngine;



using System.Collections;


namespace HumanBuilders {

  /// <summary>
  /// A component for killing the player.
  /// </summary>
  public class Death : MonoBehaviour {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// How long to prevent player input after respawn (in seconds)
    /// </summary>
    private const float RESPAWN_DELAY = 1f;

    /// <summary>
    /// A reference to the player.
    /// </summary>
    private PlayerCharacter player;

    /// <summary>
    /// The player's sprite.
    /// </summary>
    private SpriteRenderer playerSprite;

    /// <summary>
    /// Whether or not the player is currently dead.
    /// </summary>
    private bool isDead;
    #endregion


    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      player = GetComponent<PlayerCharacter>();
      playerSprite = player.GetComponent<SpriteRenderer>();
    }

    #endregion

    #region Public API

    /// <summary>
    /// Kill the player.
    /// </summary>
    public void Die() {
      Instantiate(
        player.EffectsSettings.DeathEffect,
        player.Physics.Position,
        Quaternion.identity
      );
      
      playerSprite.enabled = false;

      TransitionManager.Wipe();

      player.Physics.Disable();

      if (player.CarriedItem != null) {
        player.CarriedItem.OnPutDown();
      }

      player.DisableCrouch(this);
      player.DisableJump(this);
      player.DisableMove(this);

      isDead = true;
    }


    public void Respawn() {
      playerSprite.enabled = true;
      player.Physics.Enable();

      GameManager.Instance.resets.Reset();

      try {
        Vector3 position = TransitionManager.GetCurrentSpawnPosition();
        if (position != Vector3.positiveInfinity) {
          player.Physics.Position = position;
        }
        
        player.Physics.Velocity = Vector2.zero;
        
        bool facingRight = TransitionManager.GetCurrentSpawnFacing();
        Facing facing = facingRight ? Facing.Right : Facing.Left;
        player.SetFacing(facing);
      } catch (UnityException e) {
        Debug.Log(e);
      }

      StartCoroutine(_WaitToEnableControls());
    }

    private IEnumerator _WaitToEnableControls() {
      yield return new WaitForSeconds(RESPAWN_DELAY);

      player.EnableCrouch(this);
      player.EnableJump(this);
      player.EnableMove(this);

      isDead = false;
    }

    public bool IsDead() {
      return isDead;
    }

    #endregion
  }
}