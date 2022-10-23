#if UNITY_EDITOR

using XNode;

namespace HumanBuilders.Editor {
  public class NodeCompletenessReport : AutoNodeReport {
    public int TotalPorts { get => totalPorts; }
    public int TotalOutputPorts { get => totalOutputPorts; }
    public int TotalInputPorts { get => totalInputPorts; }
    public bool Complete { get => complete; }
    public int TotalUnconnectedPorts { get => totalUnconnected; }

    private int totalPorts;
    private int totalOutputPorts;
    private int totalInputPorts;
    private bool complete;
    private int totalUnconnected;

    public NodeCompletenessReport(IAutoNode n) : base(n) {
      totalPorts = 0;
      totalInputPorts = 0;
      totalOutputPorts = 0;

      foreach (NodePort port in node.Ports) {
        totalPorts++;
        totalOutputPorts += port.IsOutput ? 1 : 0;
        totalInputPorts += port.IsInput ? 1 : 0;
      }

      complete = node.IsNodeComplete();
      totalUnconnected = Complete ? 0 : node.TotalDisconnectedPorts();
    }

    protected override string BuildMessage() {
      if (!complete) {
        string[] typeParts = node.GetType().FullName.Split('.');
        string nodeClassName = typeParts[typeParts.Length-1];
        return string.Format("{0}: {1} unconnected ports", nodeClassName, totalUnconnected);
      }
      return "";
    }
  }
}
#endif