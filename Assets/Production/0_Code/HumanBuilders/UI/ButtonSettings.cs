using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class ButtonSettings : MonoBehaviour {

    /// <summary>
    /// The name of the sound to play when the button is being hovered over.
    /// </summary>
    public Sound HoverSound { get { return hoverSound; } }

    /// <summary>
    /// The name of the sound to play when the button is clicked.
    /// </summary>
    public Sound ClickSound { get { return clickSound; } }

    /// <summary>
    /// The name of the sound to play when the button is being hovered over.
    /// </summary>
    [Tooltip("The name of the sound to play when the button is being hovered over.")]
    [SerializeField]
    private Sound hoverSound;

    /// <summary>
    /// The name of the sound to play when the button is clicked.
    /// </summary>
    [Tooltip("The name of the sound to play when the button is clicked.")]
    [SerializeField]
    private Sound clickSound;
  }

}