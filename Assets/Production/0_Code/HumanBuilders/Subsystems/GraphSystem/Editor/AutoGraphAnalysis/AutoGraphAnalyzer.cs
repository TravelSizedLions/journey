#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  public class AutoGraphAnalyzer<T> where T: AutoGraphAnalysis {
    virtual public T Analyze(IAutoNode node) {
      throw new System.Exception("NotImplemented");
    }
  }
}
#endif