using UnityEngine;

namespace HumanBuilders {
  public class Sojourn {
    public static bool Initialized { get => Instance?.narrator != null; }
    public static bool HasQuest { get => Instance?.narrator?.Quest != null; }
    public static QuestGraph Quest { get => Instance?.narrator?.Quest; }
    public static bool Finished { get => (bool)Instance?.graphEngine?.IsFinished(); }
    private static Sojourn Instance { 
      get {
        if (_inst == null) {
          _inst = new Sojourn();
        }

        return _inst;
      } 
    }

    private static Sojourn _inst;

    private Narrator narrator;
    private GraphEngine graphEngine;
    private GameObject gameObject;


    private Sojourn() {
      gameObject = new GameObject("sojourn");
      graphEngine = gameObject.AddComponent<GraphEngine>();
    }

    public static void LoadNarrator(string path) => Instance.LoadNarrator_Inner(path);
    public void LoadNarrator_Inner(string path) {
      narrator = Resources.Load<Narrator>(path);
      narrator.SetGraphEngine(graphEngine);
    }

    public static void LoadQuest(string path) => Instance.LoadQuest_Inner(path);
    public void LoadQuest_Inner(string path) {
      if (Initialized) {
        QuestGraph quest = Resources.Load<QuestGraph>(path);
        narrator.SetQuest(quest);
      }
    }

    public static void Commence() => Instance.narrator.Commence();

    public static void CheckProgress() => Instance.narrator.CheckProgress();
  }
}