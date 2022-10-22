#if UNITY_EDITOR

using System;
using System.Collections.Generic;

namespace HumanBuilders.Editor {
  public class GraphCompletenessReport: AutoGraphReport<NodeCompletenessReport>  {

    public bool IsComplete { get => isComplete; }
    private bool isComplete;

    public GraphCompletenessReport(IAutoGraph graph) : base(graph) {
      isComplete = NodeAnalyses.TrueForAll((NodeCompletenessReport a) => a.Complete);
    }
  }
}
#endif