using System;
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
    [AutoTable(typeof(Triggerable), "World Triggers On Availability", NodeColors.START_COLOR)]
    public List<Triggerable> AvailabilityTriggers {
      get => ((QuestGraph)graph).AvailabilityTriggers;
      set => ((QuestGraph)graph).AvailabilityTriggers = value;
    }
    // [GUIColor("GetColor")]
    // [ShowInInspector]
    // [TableList]
    // public List<QuestPiece<Triggerable>> AvailabilityTriggers {
    //   get;
    //   set;
    // }

    [ShowInInspector]
    [FoldoutGroup("Triggers")]
    [AutoTable(typeof(Triggerable), "World Triggers On Start", NodeColors.START_COLOR)]
    public List<Triggerable> StartTriggers {
      get => ((QuestGraph)graph).StartTriggers;
      set => ((QuestGraph)graph).StartTriggers = value;
    }
    // [GUIColor("GetColor")]
    // [ShowInInspector]
    // [TableList]
    // public List<QuestPiece<Triggerable>> StartTriggers {
    //   get; 
    //   set;
    // }

    [ShowInInspector]
    [FoldoutGroup("Extra Quest Conditions")]
    [AutoTable(typeof(ScriptableCondition), "Additional Quest Availability Conditions", NodeColors.START_COLOR)]
    public List<ScriptableCondition> AvailabilityConditions {
      get => ((QuestGraph)graph).AvailabilityConditions;
      set => ((QuestGraph)graph).AvailabilityConditions = value;
    }
    // [GUIColor("GetColor")]
    // [ShowInInspector]
    // [TableList]
    // public List<QuestPiece<ScriptableCondition>> AvailabilityConditions {
    //   get;
    //   set;
    // }

    [ShowInInspector]
    [FoldoutGroup("Extra Quest Conditions")]
    [AutoTable(typeof(ScriptableCondition), "Additional Start Conditions", NodeColors.START_COLOR)]
    public List<ScriptableCondition> StartConditions {
      get => ((QuestGraph)graph).StartConditions;
      set => ((QuestGraph)graph).StartConditions = value;
    }
    // [GUIColor("GetColor")]
    // [ShowInInspector]
    // [AutoTable(typeof(ScriptableCondition), "Additional Start Conditions", NodeColors.START_COLOR)]
    // public List<ScriptableCondition> StartConditions {
    //   get;
    //   set;
    // }
    // public Color GetColor() => new Color(.427f, .831f, .624f);

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
    public void AddAvailabilityCondition(ScriptableCondition condition) {
      AvailabilityConditions = AvailabilityConditions ?? new List<ScriptableCondition>();
      AvailabilityConditions.Add(condition);
    }

    public void AddStartCondition(ScriptableCondition condition) {
      StartConditions = StartConditions ?? new List<ScriptableCondition>();
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
