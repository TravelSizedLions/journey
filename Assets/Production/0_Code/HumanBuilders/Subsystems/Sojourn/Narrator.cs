using UnityEngine;

namespace HumanBuilders {
  [CreateAssetMenu(fileName="New Quest Narrator", menuName="Sojourn/Quest Narrator", order=1)]
  public class Narrator : ScriptableObject {

    public QuestGraph Quest { get => (QuestGraph)graphEngine?.GetCurrentGraph(); }
    private GraphEngine graphEngine;

    public void SetGraphEngine(GraphEngine engine) {
      graphEngine = engine;
    }

    public void SetQuest(QuestGraph quest) {
      graphEngine.StartGraph(quest);
    }

    public void Commence() {
      Quest.Start();
      CheckProgress();
    }

    public void CheckProgress() {
      graphEngine.Continue();
    }

  }
}
