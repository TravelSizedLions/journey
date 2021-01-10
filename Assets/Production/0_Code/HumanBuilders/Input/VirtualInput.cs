using UnityEngine;

namespace HumanBuilders {
  
  /// <summary>
  /// This class is used to report player input outside of the Unity framework
  /// a virtual gamepad.
  /// </summary>
  public class VirtualInput : InputComponent {

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The virtual gamepad to use.
    /// </summary>
    private VirtualGamepad gamepad;
    #endregion


    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------

    /// <summary>
    /// Set the gamepad to use for collecting inputs.
    /// </summary>
    /// <param name="gamepad">The gamepad to use for inputs.</param>
    public void SetGamepad(VirtualGamepad gamepad) {
      this.gamepad = gamepad;
    }

    /// <summary>
    /// Checks if the player is holding down a certain button
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player is holding down a certain button.</returns>
    public override bool GetButton(string input) => gamepad == null ? GameManager.GamePad.GetButton(input) : gamepad.GetButton(input);

    /// <summary>
    /// Checks if the player has pressed a certain button within the current frame.
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player has pressed a certain button within the
    /// current frame.</returns>
    public override bool GetButtonDown(string input) => gamepad == null ? GameManager.GamePad.GetButtonDown(input) : gamepad.GetButtonDown(input);

    /// <summary>
    /// Checks if the player has released a certain button within the current frame.
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player has released a certain button within the
    /// current frame.</returns>
    public override bool GetButtonUp(string input) => gamepad == null ? GameManager.GamePad.GetButtonUp(input) : gamepad.GetButtonUp(input);

    /// <summary>
    /// Get the horizontal input axis.
    /// </summary>
    /// <returns>The horizontal input, from -1 to 1.</returns>
    public override float GetHorizontalInput() => gamepad == null ? GameManager.GamePad.HorizontalAxis : gamepad.HorizontalAxis;
    

    /// <summary>
    /// Get the vertical input axis.
    /// </summary>
    /// <returns>The vertical input, from -1 to 1.</returns>
    public override float GetVerticalInput() => gamepad == null ? GameManager.GamePad.VerticalAxis : gamepad.VerticalAxis;

    
    /// <summary>
    /// Gets the mouse position on the screen.
    /// </summary>
    /// <returns>The mouse position onthe screen.</returns>
    public override Vector3 GetMouseScreenPosition() => Input.mousePosition;

    /// <summary>
    /// Gets the mouse position in the world.
    /// </summary>
    /// <returns>The mouse position in the world</returns>
    public override Vector3 GetMouseWorldPosition() {
      if (GameManager.CurrentCamera == null) {
        return Vector3.zero;
      }
      
      Vector3 mouse = Input.mousePosition;
      mouse.z = 1;
      mouse = GameManager.CurrentCamera.ScreenToWorldPoint(mouse);
      return mouse;
    }

    #endregion
  }
}

