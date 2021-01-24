using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace HumanBuilders {
  public class ButtonSettings : MonoBehaviour {

    /// <summary>
    /// The name of the sound to play when the button is being hovered over.
    /// </summary>
    public string HoverSound { get { return hoverSound; } }

    /// <summary>
    /// The name of the sound to play when the button is clicked.
    /// </summary>
    public string ClickSound { get { return clickSound; } }

    /// <summary>
    /// The name of the sound to play when the button is being hovered over.
    /// </summary>
    [Tooltip("The name of the sound to play when the button is being hovered over.")]
    [ValueDropdown("FindSoundsInScene")]
    [SerializeField]
    [LabelText("Hover Sound")]
    private string hoverSound = AudioManager.DEFAULT_SOUND;

    /// <summary>
    /// The name of the sound to play when the button is clicked.
    /// </summary>
    [Tooltip("The name of the sound to play when the button is clicked.")]
    [ValueDropdown("FindSoundsInScene")]
    [SerializeField]
    [LabelText("Click Sound")]
    private string clickSound = AudioManager.DEFAULT_SOUND;


    /// <summary>
    /// Find all sounds added to sound libraries in this scene.
    /// </summary>
    private List<string> FindSoundsInScene() => AudioManager.FindSoundsInScene();
  }

}