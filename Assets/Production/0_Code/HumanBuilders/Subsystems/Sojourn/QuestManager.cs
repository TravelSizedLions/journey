using UnityEngine;

namespace HumanBuilders {
  public class QuestManager {
    public static bool Initialized { get => Instance?.runner != null; }
    public static bool HasQuest { get => Instance?.runner?.Quest != null; }
    private static QuestManager Instance { 
      get {
        if (_inst == null) {
          _inst = new QuestManager();
        }

        return _inst;
      } 
    }

    private static QuestManager _inst;


    private QuestRunner runner;
    private GraphEngine graphEngine;
    private GameObject gameObject;


    private QuestManager() {
      gameObject = new GameObject("sojourn");
      graphEngine = gameObject.AddComponent<GraphEngine>();
    }

    public static void InitQuestRunner(string path) => Instance.InitQuestRunner_Inner(path);
    public void InitQuestRunner_Inner(string path) {
      runner = Resources.Load<QuestRunner>(path);
      runner.SetGraphEngine(graphEngine);
    }

    public static void InitQuest(string path) => Instance.InitQuest_Inner(path);
    public void InitQuest_Inner(string path) {
      if (Initialized) {
        QuestAsset quest = Resources.Load<QuestAsset>(path);
        runner.SetQuest(quest);
      }
    }

  }
}