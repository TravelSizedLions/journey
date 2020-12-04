using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Storm.Subsystems.Dialog {

  /// <summary>
  /// This object is an Asset that can be used during in-game cutscenes to have
  /// something typed out on the dialog box.
  /// </summary>
  [CreateAssetMenu(fileName = "New Message", menuName = "Dialog/Message", order = 1)]
  public class Message : ScriptableObject {
    /// <summary>
    /// The person saying the sentence.
    /// </summary>
    public string Speaker;

    /// <summary>
    /// The sentence to type.
    /// </summary>
    [Multiline(4)]
    public string Sentence;
  }

}