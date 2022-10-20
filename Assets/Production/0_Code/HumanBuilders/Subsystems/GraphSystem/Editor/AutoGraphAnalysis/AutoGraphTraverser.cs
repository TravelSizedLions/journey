#if UNITY_EDITOR
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace HumanBuilders.Editor {
  // Strategy Pattern Class for traversing over all scenes in the build to
  // accomplish some task.
  public static class AutoGraphTraverser {
    // Iterates over all nodes in autograph.
    public static List<T> TraverseGraph<T>(AutoGraph graph, AutoGraphAnalyzer<T> analyzer) where T : AutoGraphAnalysis {
      List<T> analyses = new List<T>();
      graph.AutoNodes.ForEach((IAutoNode node) => analyses.Add(analyzer.Analyze(node)));
      return analyses;
    }
  }
}
#endif