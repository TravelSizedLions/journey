using UnityEngine;

namespace HumanBuilders {

  /// <summary>
  /// This object is an Asset that can be used during in-game cutscenes to have
  /// something typed out on the dialog box.
  /// </summary>
  [CreateAssetMenu(fileName = "New Message", menuName = "Dialog/Message", order=2)]
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

    /// <summary>
    /// How fast to type the message, in characters per second.
    /// </summary>
    public float Speed;
  }

}