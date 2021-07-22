using System;
using System.Collections.Generic;

namespace HumanBuilders {
  public class GraphInfo {
    public int NumCurrentNodes { get => numNodes; }
    private int numNodes;

    public GraphInfo(int numNodes) {
      this.numNodes = numNodes;
    }
  }

  public class Unsubscriber<GraphInfo> : IDisposable {
    private List<IObserver<GraphInfo>> observers;
    private IObserver<GraphInfo> observer;
    public Unsubscriber(List<IObserver<GraphInfo>> observers, IObserver<GraphInfo> observer) {
      this.observer = observer;
      this.observers = observers;
    }

    public void Dispose() {
      if (observers.Contains(observer)) {
        observers.Remove(observer);
      }
    }
  }
}