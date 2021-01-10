using UnityEngine;

namespace HumanBuilders {
  /// <summary>
  /// An interface for MonoBehaviours. It's just a mirror for commonly used
  /// Unity properties and methods. Useful for mocking in unit tests.
  /// </summary>
  public interface IMonoBehaviour {
    Transform transform { get;}

    string name { get; set; }

    T GetComponent<T>();
  }
}