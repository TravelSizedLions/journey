using System;
using UnityEngine;
using XNode;

namespace HumanBuilders {
  public interface IJourneyNode {
    QuestProgress Progress { get; }
  }

  public interface ISkippable {
    void MarkSkipped();
  }

  [Serializable]
  public abstract class JourneyNode: AutoNode, IJourneyNode, IAutoNode {
    public QuestProgress Progress { get => progress; }
    protected QuestProgress progress;

    [HideInInspector]
    [SerializeField]
    protected bool required = true;

    protected override void OnEnable() {
      progress = QuestProgress.Unavailable;
      base.OnEnable();
    }

    public override bool IsNodeComplete() {
      foreach (NodePort port in Ports) {
        if (!port.IsConnected || port.GetConnections().Count == 0) {
          if (port.IsOutput && required) {
            return false;
          }
        }
      }

      return true;
    }
  }
}