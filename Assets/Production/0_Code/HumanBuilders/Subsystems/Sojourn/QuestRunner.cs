using UnityEngine;

namespace HumanBuilders {
  [CreateAssetMenu(fileName="New Quest Runner", menuName="Sojourn/Quest Runner", order=1)]
  public class QuestRunner : ScriptableObject {

    public QuestAsset Quest { get => (QuestAsset)graphEngine?.GetCurrentGraph(); }
    private GraphEngine graphEngine;

    public void SetGraphEngine(GraphEngine engine) {
      graphEngine = engine;
    }

    public void SetQuest(QuestAsset quest) {
      graphEngine.StartGraph(quest);
    }

  }
}
