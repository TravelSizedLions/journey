using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Storm.Inputs {
  public interface IInputComponent {

    /// <summary>
    /// Get the horizontal input axis.
    /// </summary>
    /// <returns>The horizontal input, from -1 to 1.</returns>
    /// <seealso cref="UnityInput.GetHorizontalInput" />
    /// <seealso cref="VirtualInput.GetHorizontalInput" />
    float GetHorizontalInput();

    /// <summary>
    /// Get the vertical input axis.
    /// </summary>
    /// <returns>The vertical input, from -1 to 1.</returns>
    /// <seealso cref="UnityInput.GetVerticalInput" />
    /// <seealso cref="VirtualInput.GetVerticalInput" />
    float GetVerticalInput();

    /// <summary>
    /// Checks if the player is holding down a certain button
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player is holding down a certain button.</returns>
    /// <seealso cref="UnityInput.GetButton"/>
    /// <seealso cref="VirtualInput.GetButton"/>
    bool GetButton(string input);

    /// <summary>
    /// Checks if the player has pressed a certain button within the current frame.
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player has pressed a certain button within the
    /// current frame.</returns>
    /// <seealso cref="UnityInput.GetButtonDown"/>
    /// <seealso cref="VirtualInput.GetButtonDown"/>
    bool GetButtonDown(string input);

    /// <summary>
    /// Checks if the player has released a certain button within the current frame.
    /// </summary>
    /// <param name="input">The name of the button to check (i.e. "Jump,"
    /// "Fire," etc.</param>
    /// <returns>True if the player has released a certain button within the
    /// current frame.</returns>
    /// <seealso cref="UnityInput.GetButtonUp" />
    /// <seealso cref="VirtualInput.GetButtonUp" />
    bool GetButtonUp(string input);

    /// <summary>
    /// Gets the mouse position on the screen.
    /// </summary>
    /// <returns>The mouse position onthe screen.</returns>
    /// <seealso cref="UnityInput.GetMouseScreenPosition"/>
    /// <seealso cref="VirtualInput.GetMouseScreenPosition"/>
    Vector3 GetMouseScreenPosition();

    /// <summary>
    /// Gets the mouse position in the world.
    /// </summary>
    /// <returns>The mouse position in the world</returns>
    /// <seealso cref="UnityInput.GetMouseWorldPosition"/>
    /// <seealso cref="VirtualInput.GetMouseWorldPosition"/>
    Vector3 GetMouseWorldPosition();
  }

  public abstract class InputComponent : IInputComponent {
    public virtual bool GetButton(string input) {
      throw new System.NotImplementedException();
    }

    public virtual bool GetButtonDown(string input) {
      throw new System.NotImplementedException();
    }

    public virtual bool GetButtonUp(string input) {
      throw new System.NotImplementedException();
    }

    public virtual float GetHorizontalInput() {
      throw new System.NotImplementedException();
    }

    public virtual float GetVerticalInput() {
      throw new System.NotImplementedException();
    }

    /// <summary>
    /// Gets the mouse position on the screen.
    /// </summary>
    /// <returns>The mouse position onthe screen.</returns>
    public virtual Vector3 GetMouseScreenPosition() {
      return Input.mousePosition;
    }

    /// <summary>
    /// Gets the mouse position in the world.
    /// </summary>
    /// <returns>The mouse position in the world</returns>
    public virtual Vector3 GetMouseWorldPosition() {
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