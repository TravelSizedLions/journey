using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace HumanBuilders {
  public class Journey {
    public static bool Initialized { get => Instance?.narrator != null; }
    public static bool HasQuest { get => Instance?.narrator?.Quest != null; }
    public static QuestGraph Quest { get => Instance?.narrator?.Quest; }
    public static JourneyNode CurrentNode { get => (JourneyNode)Instance?.graphEngine?.GetCurrentNode(); }
    public static List<JourneyNode> CurrentNodes { get => Instance.graphEngine.GetCurrentNodes().Cast<JourneyNode>().ToList(); }
    public static bool Finished { get => (bool)Instance?.graphEngine?.IsFinished(); }
    private static Journey Instance { 
      get {
        if (_inst == null) {
          _inst = new Journey();
        }

        return _inst;
      } 
    }

    private static Journey _inst;

    private Narrator narrator;
    private GraphEngine graphEngine;
    private GameObject gameObject;


    private Journey() {
      gameObject = new GameObject("journey");
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

    public static void Begin() => Instance.narrator.Begin();

    public static void Step() => Instance.narrator.CheckProgress();
  }
}