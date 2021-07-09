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
    // Ports
    //-------------------------------------------------------------------------
    [SerializeField]
    [FoldoutGroup("Extra Quest Conditions")]
    [Tooltip("An optional list of conditions that the player is required to meet in order to make the quest discoverable in the world.")]
    [ConditionTable("Additional Quest Availability Conditions", 0.227f, 0.631f, 0.424f)]
    private List<ConditionTableEntry> AvailabilityConditions = null;

    [Space(10)]
    [SerializeField]
    [FoldoutGroup("Extra Quest Conditions")]
    [Tooltip("An optional list of conditions that the player is required to meet in order to officially start this quest.")]
    [ConditionTable("Additional Start Conditions", 0.227f, 0.631f, 0.424f)]
    private List<ConditionTableEntry> StartConditions = null;

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
      AvailabilityConditions = AvailabilityConditions ?? new List<ConditionTableEntry>();
      AvailabilityConditions.Add(new ConditionTableEntry(condition));
    }

    public void AddStartCondition(ICondition condition) {
      StartConditions = StartConditions ?? new List<ConditionTableEntry>();
      StartConditions.Add(new ConditionTableEntry(condition));
    }

    public bool CanBecomeAvailable() {
      if (AvailabilityConditions != null) {
        foreach (var entry in AvailabilityConditions) {
          if (!entry.Condition.IsMet()) {
            return false;
          }
        }
      }

      return progress == QuestProgress.Unavailable;
    }

    public bool CanStart() {
      if (StartConditions != null) {
        foreach (var entry in StartConditions) {
          if (!entry.Condition.IsMet()) {
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
