using System;
using System.Collections.Generic;
using System.Linq;
using FJSON;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders {
  public class Journey : IObserver<GraphInfo> {
    private const string QUEST_PROGRESS = "quest_progress";
    private const string QUEST_PATH = "quest_path";
    private const string STEP_COUNT = "step_count";
    public static bool Initialized { get => Instance.narrator != null; }
    public static bool HasQuest { get => Instance.narrator?.Quest != null; }
    public static QuestGraph Quest { get => Instance.narrator?.Quest; }
    public static JourneyNode CurrentNode { get => (JourneyNode) Instance?.graphEngine?.GetCurrentNode(); }
    public static List<JourneyNode> CurrentNodes { get => Instance.graphEngine.GetCurrentNodes().Cast<JourneyNode>().ToList(); }
    public static bool Finished { get => Instance.graphEngine.IsFinished(); }
    public static int StepCount { get => Instance.stepCount; }
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
    private int stepCount;
    private string questPath;

    private Journey() {
      gameObject = new GameObject("journey");
      graphEngine = gameObject.AddComponent<GraphEngine>();
      graphEngine.Subscribe(this);
    }

    public static void SetNarrator(Narrator narr) => Instance.SetNarrator_Inner(narr);
    private void SetNarrator_Inner(Narrator n) {
      narrator = n;
      narrator.SetGraphEngine(graphEngine);
    }

    public static void LoadNarrator(string path) => Instance.LoadNarrator_Inner(path);
    private void LoadNarrator_Inner(string path) {
      narrator = Resources.Load<Narrator>(path);
      narrator.SetGraphEngine(graphEngine);
    }

    public static void LoadQuest(string path) => Instance.LoadQuest_Inner(path);
    private void LoadQuest_Inner(string path) {
      if (Initialized) {
        QuestGraph quest = Resources.Load<QuestGraph>(path);
        SetQuest(quest);
        questPath = path;
      }
    }

    public static void SetQuest(QuestGraph quest) => Instance.SetQuest_Inner(quest);
    private void SetQuest_Inner(QuestGraph quest) {
      narrator.SetQuest(quest);
      stepCount = 0;
    }

    public static void Begin() => Instance.narrator.Begin();

#if UNITY_EDITOR
    [MenuItem("Journey/Step #s")]
#endif
    public static void Step() => Instance.narrator.CheckProgress();

    public static void SaveProgress() => Instance.SaveProgress_Inner();
    private void SaveProgress_Inner() {
      if (Quest == null) {
        questPath = "";
      }

      VSave.Set(StaticFolders.QUEST_DATA, QUEST_PATH, questPath);

      List<string> json = SerializeNodes(CurrentNodes);
      for (int i = 0; i < json.Count; i++) {
        string j = json[i];
        VSave.Set(StaticFolders.QUEST_DATA, QUEST_PROGRESS+"_"+i, j);
      }

      VSave.Set(StaticFolders.QUEST_DATA, QUEST_PROGRESS+"_num_current", json.Count);
      VSave.Set(StaticFolders.QUEST_DATA, STEP_COUNT, stepCount);

      VSave.Save();
    }

    private List<string> SerializeNodes(List<JourneyNode> nodes) {
      List<string> nodeJson = new List<string>();
      foreach (var node in nodes) {
        nodeJson.Add(JSON.ToNiceJSON(node));
      }

      return nodeJson;
    }

    public static void Resume() => Instance.Resume_Inner();
    public void Resume_Inner() {
      if (string.IsNullOrEmpty(questPath)) {
        if (VSave.Get(StaticFolders.QUEST_DATA, QUEST_PATH, out string p)) {
          questPath = p;
        }
      }

      if (!string.IsNullOrEmpty(questPath)) {
        LoadQuest(questPath);
      }

      if (VSave.Get(StaticFolders.QUEST_DATA, QUEST_PROGRESS, out string json)) {
        List<IAutoNode> nodes = DeserializeNodes();
        graphEngine.SetCurrentNodes(nodes);
      }

      if (VSave.Get(StaticFolders.QUEST_DATA, STEP_COUNT, out int count)) {
        stepCount = count;
      }
    }

    private List<IAutoNode> DeserializeNodes() {
      int numCurrent = VSave.Get<int>(StaticFolders.QUEST_DATA, QUEST_PROGRESS+"_num_current");
      List<IAutoNode> nodes = new List<IAutoNode>();
      for(int i = 0; i < numCurrent; i++) {
        string json = VSave.Get<string>(StaticFolders.QUEST_DATA, QUEST_PROGRESS+"_"+i);
        JourneyNode node = (JourneyNode)JSON.ToObject(json);
        nodes.Add(node);
      }

      return nodes;
    }

    public static void Reset() => Instance.Reset_Inner();
    private void Reset_Inner() {
      questPath = null;
      narrator = null;
      stepCount = 0;
    }

    public void OnError(Exception error) {}

    public void OnCompleted() {}

    public void OnNext(GraphInfo value) => stepCount++;
  }
}