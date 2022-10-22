#if UNITY_EDITOR

using System;
using System.Collections.Generic;

namespace HumanBuilders.Editor {
  public class AutoGraphReport<T> where T: AutoNodeReport  {

    public List<T> NodeAnalyses;

    public AutoGraphReport(IAutoGraph graph) {
      NodeAnalyses = TraverseGraph(graph);
    }


    private List<T> TraverseGraph(IAutoGraph graph) {
      List<T> analyses = new List<T>();
      graph.AutoNodes.ForEach((IAutoNode node) => analyses.Add((T)Activator.CreateInstance(typeof(T), node)));
      return analyses;
    }
  }
}
#endif