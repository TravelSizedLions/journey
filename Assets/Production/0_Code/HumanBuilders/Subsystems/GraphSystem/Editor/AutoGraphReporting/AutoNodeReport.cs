#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  public abstract class AutoNodeReport: Report  {
    public IAutoNode Node { get => node; }
    protected IAutoNode node;

    public AutoNodeReport(IAutoNode n) {
      node = n;
    }
  }
}
#endif