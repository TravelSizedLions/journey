using HumanBuilders.Graphing;
using UnityEngine;

namespace HumanBuilders {
  [CreateAssetMenu(fileName="New Quest Narrator", menuName="Journey/Quest Narrator", order=1)]
  public class Narrator : ScriptableObject {

    public QuestGraph Quest { get => (QuestGraph)graphEngine?.GetCurrentGraph(); }
    private GraphEngine graphEngine;

    public void SetGraphEngine(GraphEngine engine) {
      graphEngine = engine;
    }

    public void SetQuest(QuestGraph quest) {
      graphEngine?.SetCurrentGraph((IAutoGraph)quest);
    }

    public void Begin() {
      IAutoGraph graph = graphEngine?.GetCurrentGraph();
      if (graph != null) {
        graphEngine?.StartGraph(graph);
      } else {
        Debug.LogWarning("Set the quest using Journey.SetQuest() before beginning.");
      }
    }

    public void CheckProgress() {
      graphEngine.Continue();
    }

  }
}
