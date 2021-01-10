using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// This class pulls input from Unity's Input system.
  /// </summary>
  public class UnityInput : InputComponent {

    /// <summary>
    /// Checks if the player is holding down a certain button
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player is holding down a certain button.</returns>
    public override bool GetButton(string input) {
      return Input.GetButton(input);
    }

    /// <summary>
    /// Checks if the player has pressed a certain button within the current frame.
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player has pressed a certain button within the
    /// current frame.</returns>
    public override bool GetButtonDown(string input) {
      return Input.GetButtonDown(input);
    }

    /// <summary>
    /// Checks if the player has released a certain button within the current frame.
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player has released a certain button within the
    /// current frame.</returns>
    public override bool GetButtonUp(string input) {
      return Input.GetButtonUp(input);
    }

    /// <summary>
    /// Get the horizontal input axis.
    /// </summary>
    /// <returns>The horizontal input, from -1 to 1.</returns>
    public override float GetHorizontalInput() {
      return Input.GetAxis("Horizontal");
    }
    

    /// <summary>
    /// Get the vertical input axis.
    /// </summary>
    /// <returns>The vertical input, from -1 to 1.</returns>
    public override float GetVerticalInput() {
      return Input.GetAxis("Vertical");
    }
    
    /// <summary>
    /// Gets the mouse position on the screen.
    /// </summary>
    /// <returns>The mouse position onthe screen.</returns>
    public override Vector3 GetMouseScreenPosition() {
      return Input.mousePosition;
    }

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
  }

}

