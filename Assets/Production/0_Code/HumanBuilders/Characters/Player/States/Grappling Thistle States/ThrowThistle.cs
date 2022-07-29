using System.Collections;
using System.Collections.Generic;


using UnityEngine;


namespace HumanBuilders {

  /// <summary>
  /// When the player is standing still.
  /// </summary>
  public class ThrowThistle : PlayerState {
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    public override string AnimParam { get { return param; } }

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The trigger parameter for this state.
    /// </summary>
    private string param = "throw_thistle";

    /// <summary>
    /// Fires once per frame. Use this instead of Unity's built in Update() function.
    /// </summary>
    public override void OnUpdate() {

    }

    /// <summary>
    /// Fires with every physics tick. Use this instead of Unity's built in FixedUpdate() function.
    /// </summary>
    public override void OnFixedUpdate() {

    }
    
    /// <summary>
    ///  Fires whenever the state is entered into, after the previous state exits.
    /// </summary>
    public override void OnStateEnter() {
      physics.Velocity = Vector2.zero;
    }

    /// <summary>
    /// Fires when code outside the state machine is trying to send information.
    /// </summary>
    /// <param name="signal">The signal sent.</param>
    public override void OnSignal(GameObject obj) {

    }
  }
}