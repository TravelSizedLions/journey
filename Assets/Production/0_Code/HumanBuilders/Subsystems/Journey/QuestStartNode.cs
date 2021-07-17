using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using XNode;

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
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(ITriggerable), "World Triggers On Availability", NodeColors.START_COLOR)]
    public AutoTable<ITriggerable> AvailabilityTriggers {
      get => ((QuestGraph)graph).AvailabilityTriggers;
      set => ((QuestGraph)graph).AvailabilityTriggers = value;
    }

    [ShowInInspector]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(ITriggerable), "World Triggers On Start", NodeColors.START_COLOR)]
    public AutoTable<ITriggerable> StartTriggers {
      get => ((QuestGraph)graph).StartTriggers;
      set => ((QuestGraph)graph).StartTriggers = value;
    }

    [ShowInInspector]
    [FoldoutGroup("Extra Quest Conditions")]
    [AutoTable(typeof(ICondition), "Additional Quest Availability Conditions", NodeColors.START_COLOR)]
    public AutoTable<ICondition> AvailabilityConditions {
      get => ((QuestGraph)graph).AvailabilityConditions;
      set => ((QuestGraph)graph).AvailabilityConditions = value;
    }

    [ShowInInspector]
    [FoldoutGroup("Extra Quest Conditions")]
    [AutoTable(typeof(ICondition), "Additional Start Conditions", NodeColors.START_COLOR)]
    public AutoTable<ICondition> StartConditions {
      get => ((QuestGraph)graph).StartConditions;
      set => ((QuestGraph)graph).StartConditions = value;
    }

    //-------------------------------------------------------------------------
    // AutoNode API
    //-------------------------------------------------------------------------
    public override void Handle(GraphEngine graphEngine) {
      QuestGraph quest = (QuestGraph)graph;
      if (CanBecomeAvailable()) {
        progress = QuestProgress.Available;
        quest.MakeAvailable();
      } 
      
      if (CanStart()) {
        progress = QuestProgress.Completed;
        quest.Start();
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
    public void AddAvailabilityCondition(ICondition condition) {
      AvailabilityConditions = AvailabilityConditions ?? new AutoTable<ICondition>();
      AvailabilityConditions.Add(condition);
    }

    public void AddStartCondition(ICondition condition) {
      StartConditions = StartConditions ?? new AutoTable<ICondition>();
      StartConditions.Add(condition);
    }

    public bool CanBecomeAvailable() {
      if (AvailabilityConditions != null) {
        foreach (var condition in AvailabilityConditions) {
          if (!condition.IsMet()) {
            return false;
          }
        }
      }

      // Debug.Log("Can be available check: " + (progress == QuestProgress.Unavailable));
      return progress == QuestProgress.Unavailable;
    }

    public bool CanStart() {
      if (StartConditions != null) {
        foreach (var condition in StartConditions) {
          if (!condition.IsMet()) {
            return false;
          }
        }
      }

      // Debug.Log("Can start check: " + (progress == QuestProgress.Available));
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
