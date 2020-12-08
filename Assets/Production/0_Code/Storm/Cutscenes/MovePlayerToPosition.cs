using System.Collections;
using System.Collections.Generic;
using Storm;
using Storm.Characters.Player;
using Storm.Inputs;
using UnityEngine;
using UnityEngine.Playables;

namespace Storm.Cutscenes {
  public class MovePlayerToPosition : MonoBehaviour {
    #region Fields
    /// <summary>
    /// How fast the player should move towards their target. 0 - No movement, 
    /// 1 - Move at the max player speed.
    /// </summary>
    [SerializeField]
    [Tooltip("How fast the player should move towards their target. 0 - No movement, 1 - Move at the max player speed.")]
    [Range(0, 1)]
    private float inputSpeed;

    /// <summary>
    /// Whether or not the player has started walking.
    /// </summary>
    private bool walking;

    /// <summary>
    /// Whether or not the player should move to the left or to the right to
    /// reach the target.
    /// </summary>
    private bool walkLeft;

    /// <summary>
    /// The position the character should move to.
    /// </summary>
    private Transform target;

    /// <summary>
    /// The virtual gamepad used to move the player vicariously.
    /// </summary>
    private VirtualGamepad gamepad;

    /// <summary>
    /// The virtual input to hand the player during manual movement.
    /// </summary>
    private IInputComponent virtualInput;

    /// <summary>
    /// Used to cache the real player input component while the player is being
    /// controlled with the virtual one.
    /// </summary>
    private IInputComponent playerInput;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------
    private void Awake() {
      gamepad = gameObject.AddComponent<VirtualGamepad>();
      VirtualInput input = new VirtualInput();
      input.SetGamepad(gamepad);
      virtualInput = input;
    }
    
    private void Update() {
      if (target != null && !walking) {
        float input = walkLeft ? -inputSpeed : inputSpeed;
        gamepad.SetHorizontalAxis(input);
        walking = true;
      }

      if (walking && ShouldStop()) {
        walking = false;
        target = null;
        gamepad.SetHorizontalAxis(0);
        GameManager.Player.PlayerInput = playerInput;
      }
    }


    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void MovePlayerTo(Transform point = null) {
      if (point == null) {
        target = transform;
      } else {
        target = point;
      }

      walkLeft = GameManager.Player.Physics.Px > target.position.x;
      playerInput = GameManager.Player.PlayerInput;
      GameManager.Player.PlayerInput = virtualInput;
    }
    #endregion


    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    
    /// <summary>
    /// Whether or not the player should stop moving towards the target.
    /// </summary>
    /// <returns></returns>
    private bool ShouldStop() {
      if (walkLeft) {
        return GameManager.Player.Physics.Px <= target.position.x; 
      } else {
        return GameManager.Player.Physics.Px >= target.position.x;
      }
    }

    #endregion

  }
}
