
using System;
using UnityEngine;

namespace HumanBuilders {
  public interface IJourneyNode {
    QuestProgress Progress { get; }
  }

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
  }
}