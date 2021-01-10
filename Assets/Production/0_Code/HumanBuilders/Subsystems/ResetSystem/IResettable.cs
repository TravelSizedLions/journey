

namespace HumanBuilders {

  /// <summary>
  /// Interface for things that can be reset. This is for MonoBehaviours that
  /// are already a subclass of something other than the Resetting class.
  /// </summary>
  public interface IResettable {
    void Reset();
  }

}