using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR
using XNodeEditor;
#endif

namespace HumanBuilders {
  [CreateNodeMenu("")]
  [NodeTint(NodeColors.START_COLOR)]
  [NodeWidth(400)]
  [DisallowMultipleNodes]
  public class QuestStartNode : JourneyNode {
    //-------------------------------------------------------------------------
    // Ports
    //-------------------------------------------------------------------------
    [Output(connectionType = ConnectionType.Multiple)]
    public EmptyConnection Output;

    //-------------------------------------------------------------------------
    // Quest Conditions
    //-------------------------------------------------------------------------
    [ShowInInspector]
    [TitleGroup("Progress Conditions")]
    [AutoTable(typeof(VCondition), "Availability Prerequisites", NodeColors.START_COLOR)]
    public List<VCondition> AvailabilityConditions {
      get => ((QuestGraph)graph).AvailabilityConditions;
      set => ((QuestGraph)graph).AvailabilityConditions = value;
    }

    [ShowInInspector]
    [TitleGroup("Progress Conditions")]
    [AutoTable(typeof(VCondition), "Start Prerequisites", NodeColors.START_COLOR)]
    public List<VCondition> StartConditions {
      get => ((QuestGraph)graph).StartConditions;
      set => ((QuestGraph)graph).StartConditions = value;
    }

    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(Triggerable), "On Quest Availability", NodeColors.START_COLOR)]
    public List<VTrigger> AvailabilityTriggers {
      get => ((QuestGraph)graph).AvailabilityTriggers;
      set => ((QuestGraph)graph).AvailabilityTriggers = value;
    }

    [ShowInInspector]
    [TitleGroup("Progress Triggers")]
    [AutoTable(typeof(Triggerable), "On Quest Start", NodeColors.START_COLOR)]
    public List<VTrigger> StartTriggers {
      get => ((QuestGraph)graph).StartTriggers;
      set => ((QuestGraph)graph).StartTriggers = value;
    }

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      QuestGraph quest = (QuestGraph)graph;
      Debug.Log("Step");
      if (CanBecomeAvailable()) {
        progress = QuestProgress.Available;
        quest.MakeAvailable();
      } 
      
      if (CanStart()) {
        progress = QuestProgress.Completed;
        quest.MarkStarted();
      }
    }

    public override void PostHandle(GraphEngine graphEngine) {
      QuestGraph quest = (QuestGraph)graph;
      if (quest.Progress == QuestProgress.Started) {
        base.PostHandle(graphEngine);
      }
    }

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void AddAvailabilityCondition(VCondition condition) {
      AvailabilityConditions = AvailabilityConditions ?? new List<VCondition>();
      AvailabilityConditions.Add(condition);
    }

    public void AddStartCondition(VCondition condition) {
      StartConditions = StartConditions ?? new List<VCondition>();
      StartConditions.Add(condition);
    }

    public bool CanBecomeAvailable() {
      if (AvailabilityConditions != null) {
        foreach (var condition in AvailabilityConditions) {
          if (!condition.IsMet()) {
            Debug.Log("Not met");
            return false;
          }
        }
      }

      QuestGraph quest = (QuestGraph)graph;
      Debug.Log("Progress: " + quest.Progress);
      return quest.Progress == QuestProgress.Unavailable;
    }

    public bool CanStart() {
      if (StartConditions != null) {
        foreach (var condition in StartConditions) {
          if (!condition.IsMet()) {
            return false;
          }
        }
      }

      return progress == QuestProgress.Available;    
    }


#if UNITY_EDITOR
    [ContextMenu("To Parent Quest")]
    public void Exit() {
      QuestGraph quest = (QuestGraph)graph;
      if (quest?.GetParent() != null) {
        NodeEditorWindow.Open(quest.GetParent());
      }
    }
#endif

  }
}
