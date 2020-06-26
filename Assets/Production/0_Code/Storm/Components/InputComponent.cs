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
  }

  public class UnityInput : IInputComponent {
    public bool GetButton(string input) {
      return Input.GetButton(input);
    }

    public bool GetButtonDown(string input) {
      return Input.GetButtonDown(input);
    }

    public bool GetButtonUp(string input) {
      return Input.GetButtonUp(input);
    }

    public float GetHorizontalInput() {
      return Input.GetAxis("Horizontal");
    }
  }
}

