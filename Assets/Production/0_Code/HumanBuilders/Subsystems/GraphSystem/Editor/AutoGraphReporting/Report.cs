#if UNITY_EDITOR

namespace HumanBuilders.Editor {
  public abstract class Report {
    public bool HasMessage { 
      get {
        if (!messageBuilt) {
          message = BuildMessage();
          messageBuilt = true;
        }
        return !string.IsNullOrEmpty(message);
      }
    }
    
    public string Message { 
      get {
        if (!messageBuilt) {
          message = BuildMessage();
          messageBuilt = true;
        }
        return message;
      } 
    }

    private string message;
    private bool messageBuilt;

    protected abstract string BuildMessage();
  }
}

#endif