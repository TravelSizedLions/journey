using UnityEngine;

namespace HumanBuilders {

  public interface IStorable {

    /// <summary>
    /// Store information about this class.
    /// </summary>
    void Store();

    /// <summary>
    /// Retrieve information about this class.
    /// </summary>
    void Retrieve();

  }
}