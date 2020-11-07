using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Storm.Components {

  public interface IPlayerInput {
    
  }

  public interface IInputComponent {
    float GetHorizontalInput();

    bool GetButton(string input);

    bool GetButtonDown(string input);

    bool GetButtonUp(string input);

    Vector3 GetMouseScreenPosition();

    Vector3 GetMouseWorldPosition();
  }

  public class UnityInput : IInputComponent {

    /// <summary>
    /// The current Scene's camera.
    /// </summary>
    private Camera camera;

    /// <summary>
    /// Checks if the player is holding down a certain button
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player is holding down a certain button.</returns>
    public bool GetButton(string input) {
      return Input.GetButton(input);
    }

    /// <summary>
    /// Checks if the player has pressed a certain button within the current frame.
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player has pressed a certain button within the
    /// current frame.</returns>
    public bool GetButtonDown(string input) {
      return Input.GetButtonDown(input);
    }

    /// <summary>
    /// Checks if the player has released a certain button within the current frame.
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player has released a certain button within the
    /// current frame.</returns>
    public bool GetButtonUp(string input) {
      return Input.GetButtonUp(input);
    }

    /// <summary>
    /// Get the horizontal input axis.
    /// </summary>
    /// <returns>The horizontal input, from -1 to 1.</returns>
    public float GetHorizontalInput() {
      return Input.GetAxis("Horizontal");
    }

    /// <summary>
    /// Gets the mouse position on the screen.
    /// </summary>
    /// <returns>The mouse position onthe screen.</returns>
    public Vector3 GetMouseScreenPosition() {
      return Input.mousePosition;
    }

    /// <summary>
    /// Gets the mouse position in the world.
    /// </summary>
    /// <returns>The mouse position in the world</returns>
    public Vector3 GetMouseWorldPosition() {
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

