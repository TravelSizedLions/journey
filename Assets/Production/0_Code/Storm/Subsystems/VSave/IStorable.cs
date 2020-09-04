using UnityEngine;

namespace Storm.Subsystems.Save {

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