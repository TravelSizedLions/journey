
using System;

namespace HumanBuilders {
  public interface IJourneyNode {
    QuestProgress Progress { get; }

  }

  public abstract class JourneyNode: AutoNode, IJourneyNode, IAutoNode {
    public QuestProgress Progress { get => progress; }
    protected QuestProgress progress;
  }
}