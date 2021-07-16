
using System;

namespace HumanBuilders {
  public interface IJourneyNode {
    QuestProgress Progress { get; }
    bool Required { get; set; }
  }

  public abstract class JourneyNode: AutoNode, IJourneyNode, IAutoNode {
    public QuestProgress Progress { get => progress; }
    protected QuestProgress progress;

    public virtual bool Required { get => required; set {} }
    protected bool required = true;
  }
}