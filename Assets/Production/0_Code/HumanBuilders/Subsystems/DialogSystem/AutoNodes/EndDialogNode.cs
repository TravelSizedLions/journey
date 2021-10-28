namespace HumanBuilders {
  [NodeTint(NodeColors.END_NODE)]
  [CreateNodeMenu("End/End Dialog")]
  public class EndDialogNode : EndNode {
    public override void Handle(GraphEngine graphEngine) => graphEngine.EndGraph();
    public override void PostHandle(GraphEngine graphEngine) {}
  }
}
