#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  public class NodeCompletenessAnalysis : AutoGraphAnalysis {
    public IAutoNode Node;
    public int TotalPorts;
    public int TotalOutputPorts;
    public int TotalInputPorts;
    public bool Complete;
    public int TotalUnconnectedPorts;
  }
}
#endif