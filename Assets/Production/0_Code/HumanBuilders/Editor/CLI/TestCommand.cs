#if UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace HumanBuilders {
  public class TestCommand {
    //-------------------------------------------------------------------------
    // Constants
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private string unityPath;


    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------
    public TestCommand() {}

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void ParseCommandLine() {
      unityPath = CLITools.GetArgument(0);

      // string command = "{0}";
    }

    public void Execute() {
      //  unity -batchmode -nographics -runTests -runSynchronously -projectPath
      //  . -logFile /j/logs/test.txt -testResults /j/logs/test.xml

      // unity -batchmode -nographics -executeCommand
      

    }

    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    private List<string> GetRelevantCommands() {
      string[] args = Environment.GetCommandLineArgs();
      List<string> relevant = new List<string>();

      for (int i = 1; i < args.Length; i++) {
        string arg = args[i];
        if (!SkipArgument(arg)) {
          relevant.Add(arg);
        }
      }

      return relevant;
    }

    private bool SkipArgument(string command) {
      command = command.ToLower();
      return !(
        command.Contains("unity") ||
        command.Contains("executecommand") ||
        command.Contains("humanbuilders.cli.test")
      );
    }
  }
}

#endif