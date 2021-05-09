using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders {

  public class ControlInputDisplay : MonoBehaviour {
    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    /// <summary>
    /// The GameObject the Inputs UI is kept under.
    /// </summary>
    [SerializeField]
    [Tooltip("The GameObject the Inputs UI is kept under.")]
    private GameObject InputGUI = null;

    /// <summary>
    /// The toggle button for this script.
    /// </summary>
    [SerializeField]
    [Tooltip("The toggle button for the script.")]
    private UnityEngine.UI.Toggle toggle = null;

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------
    private void Awake() {
      if (toggle != null) {
        if (!toggle.isOn) {
          InputGUI.SetActive(false);
        }
      }
    }

    public void Toggle() {
      InputGUI.SetActive(!InputGUI.activeSelf);
    }
  }
}