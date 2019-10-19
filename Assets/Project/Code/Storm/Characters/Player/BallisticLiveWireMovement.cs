using UnityEngine;

namespace Storm.Characters.Player {


  /// <summary>
  /// Jerrod's movement mode after being shot from a spark launcher.
  /// 
  /// Jerrod, in spark form, takes a ballistic trajectory based on directional inputs
  /// from the player. If the player presses the jump button, Jerrod reverts back to normal
  /// with a small leap. Otherwise, Jerrod returns to normal upon hitting the ground or a wall.
  ///
  /// While in the air, the player still has a little control over Jerrod's trajectory.
  /// </summary>
  public class BallisticLiveWireMovement : PlayerBehavior {
    
    #region Air Control Parameters
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    
    [Tooltip("How much control the player has over Jerrod's ascent/descent. Higher means more control.")]
    /// <summary>How much control the player has over Jerrod's ascent/descent. Higher means more control.</summary>
    public float verticalAirControl;
    
    [Tooltip("How much control the player has over Jerrod's mid air left/right movement. Higher means more control.")]
    /// <summary>How much control the player has over Jerrod's mid air left/right movement. Higher means more control.</summary>
    public float horizontalAirControl;
    #endregion Air Control Parameters
    
    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    public override void Awake() {
    
    }
    
    
    public void Start() {
    
    }
    
    
    public void Update() {
    
    }
    
    public void FixedUpdate() {
    
    }
    #endregion Unity API
    
    
    
    #region PlayerMovement API
    //-------------------------------------------------------------------------
    // PlayerMovement API
    //-------------------------------------------------------------------------
    public override void Activate() {
      base.Activate();
      
    }
    
    
    public override void Deactivate() {
      base.Deactivate();
      
    }
    
    
    public void SetInitialVelocity(Vector2 velocity) {
      rb.velocity = velocity;
    }
    #endregion PlayerMovement API
  }

}