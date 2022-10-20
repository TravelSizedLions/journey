#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  public class SceneAnalyzer<T> where T: SceneAnalysis {
    virtual public T Analyze(string scenePath) {
      throw new System.Exception("NotImplemented");
    }
  }
}
#endif