using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// A script used to mimic a gamepad. This is used during cutscenes to help
  /// direct the player in a natural way.
  /// </summary>
  public class VirtualGamepad : MonoBehaviour {

    #region Properties
    //-------------------------------------------------------------------------
    // Properties
    //-------------------------------------------------------------------------

    /// <summary>
    /// The gamepad's horizontal input.
    /// </summary>
    public float HorizontalAxis { get { return hAxis; } }

    /// <summary>
    /// The gamepad's vertical input.
    /// </summary>
    public float VerticalAxis { get { return vAxis; } }
    #endregion

    #region Fields
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------

    /// <summary>
    /// The state of a button.
    /// <list type="bullet">
    ///   <item> Off - no input </item>
    ///   <item> Press - pressed within the current frame </item>
    ///   <item> Hold - held down for 1+ frames </item>
    ///   <item> Release - released within the current frame </item>
    /// </list>
    /// </summary>
    private enum InputState { Off, Press, Hold, Release }

    /// <summary>
    /// The list of buttons and their current states.
    /// </summary>
    private Dictionary<string, InputState> buttonStates;

    /// <summary>
    /// Which inputs are slated to change on the next frame update.
    /// </summary>
    private Dictionary<string, bool> inputsToChange;

    /// <summary>
    /// The gamepad's horizontal input.
    /// </summary>
    private float hAxis;

    /// <summary>
    /// The gamepad's vertical input.
    /// </summary>
    private float vAxis;
    #endregion

    #region Unity API
    //-------------------------------------------------------------------------
    // Unity API
    //-------------------------------------------------------------------------

    private void Awake() {
      buttonStates = new Dictionary<string, InputState>();
      inputsToChange = new Dictionary<string, bool>();
    }

    private void LateUpdate() {
      foreach (string button in buttonStates.Keys) {
        if (buttonStates[button] == InputState.Press) {
          TryChangeButton(button, InputState.Hold);
        } else if (buttonStates[button] == InputState.Release) {
          TryChangeButton(button, InputState.Off);
        }
      }
    }

    #endregion

    #region Public Interface
    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    /// <summary>
    /// Press a virtual button. Once pressed, the button will be held until 
    /// <see cref="ReleaseButton"/> is called.
    /// </summary>
    /// <param name="button">The name of the button to press ("Jump", "Fire",
    /// "Menu", etc).</param>
    public void PressButton(string button) {
      if (!buttonStates.ContainsKey(button)) {
        buttonStates.Add(button, InputState.Press);
        inputsToChange.Add(button, false);
      } else if (buttonStates[button] != InputState.Hold) {
        buttonStates[button] = InputState.Press;
      }
    }

    /// <summary>
    /// Press a virtual button. Once released, the button will report being recently
    /// released for a frame, then will be considered fully Up.
    /// </summary>
    /// <param name="button">The name of the button to press ("Jump", "Fire",
    /// "Menu", etc).</param>
    public void ReleaseButton(string button) {
      if (!buttonStates.ContainsKey(button)) {
        buttonStates.Add(button, InputState.Release);
        inputsToChange.Add(button, false);
      } else if (buttonStates[button] != InputState.Off) {
        buttonStates[button] = InputState.Release;
      }
    }

    /// <summary>
    /// Sets the horizontal axis for the gamepad. This would be akin to moving
    /// the analog stick left or right.
    /// </summary>
    /// <param name="axis">The value of the axis. left is < 0, right is > 0.
    /// The range is [-1, 1].</param>
    public void SetHorizontalAxis(float axis) {
      hAxis = Mathf.Clamp(axis, -1, 1);
    }

    /// <summary>
    /// Sets the vertical axis for the gamepad. This would be akin to moving
    /// the analog stick left or right.
    /// </summary>
    /// <param name="axis">The value of the axis. up is > 0, down is < 0. The
    /// range is [-1, 1]</param>
    public void SetVerticalAxis(float axis) {
      vAxis = Mathf.Clamp(axis, -1, 1);
    }

    /// <summary>
    /// External system function used to actually set the cursor.
    /// </summary>
    /// <param name="x">The cursor's x position in screen space.</param>
    /// <param name="y">The cursor's y position in screen space.</param>
    public void SetMouseScreenPosition(int x, int y) {
      VirtualGamepad.SetCursorPos(x, y);
    }

    /// <summary>
    /// Get whether or not a button was pressed within the current frame.
    /// </summary>
    /// <param name="button">The name of the button ("Jump", "Fire",
    /// "Menu", etc).</param>
    public bool GetButtonDown(string button) {
      return buttonStates.ContainsKey(button) && buttonStates[button] == InputState.Press;
    }

    /// <summary>
    /// Get whether or not a button is down.
    /// </summary>
    /// <param name="button">The name of the button ("Jump", "Fire",
    /// "Menu", etc).</param>
    public bool GetButton(string button) {
      return buttonStates.ContainsKey(button) &&
        (buttonStates[button] == InputState.Press ||
          buttonStates[button] == InputState.Hold);
    }

    /// <summary>
    /// Get whether or not a button was released within the current frame.
    /// </summary>
    /// <param name="button">The name of the button ("Jump", "Fire",
    /// "Menu", etc).</param>
    public bool GetButtonUp(string button) {
      return !buttonStates.ContainsKey(button) ||
        buttonStates[button] == InputState.Release ||
        buttonStates[button] == InputState.Off;
    }


    #endregion

    #region Helper Methods
    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    /// <summary>
    /// Tries to update the button's state automatically. If the update is not
    /// made on the button this call, the update will happen the next call is
    /// made with the same button.
    /// </summary>
    /// <param name="button">The button to change.</param>
    /// <param name="nextState">The input state to change it to.</param>
    private void TryChangeButton(string button, InputState nextState) {
      if (inputsToChange[button]) {
        buttonStates[button] = nextState;
        inputsToChange[button] = false;
      } else {
        inputsToChange[button] = true;
      }
    }

    /// <summary>
    /// External system function used to actually set the cursor.
    /// </summary>
    /// <param name="x">The cursor's x position in screen space.</param>
    /// <param name="y">The cursor's y position in screen space.</param>
    [DllImport("user32.dll")]
    private static extern bool SetCursorPos(int x, int y);
    #endregion
  }
}