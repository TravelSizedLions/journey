#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;

namespace HumanBuilders {
  public class TestCommand {
    //-------------------------------------------------------------------------
    // Constants
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private string unityPath;
    private string resultsPath;
    private List<string> relevantCommands;

    private string command;

    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------
    public TestCommand() {}

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void ParseCommandLine() {
      unityPath = CLITools.GetArgument(0);
      resultsPath = GetResultsPath();
      relevantCommands = GetRelevantCommands();
      command = RebuildArguments(resultsPath);
    }

    public void Execute() {
      //  unity -batchmode -nographics -runTests -runSynchronously -projectPath
      //  . -logFile -testResults /j/logs/test.xml

      // unity -batchmode -nographics -executeCommand HumanBuilders.CLI.Test
      // -testResults /j/logs/test.xml
      ProcessStartInfo procInfo = new ProcessStartInfo(unityPath, command);
      procInfo.RedirectStandardOutput = true;
      procInfo.UseShellExecute = false;

      Process proc = new Process();
      proc.StartInfo = procInfo;
      proc.Start();
      proc.WaitForExit();
      
      ShowResults();
    }

    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    private void ShowResults() {
      XmlDocument doc = new XmlDocument();
      doc.Load(resultsPath);

      foreach (XmlNode node in doc.ChildNodes) {
        if (node.Attributes.GetNamedItem("name") != null && 
            node.Attributes.GetNamedItem("name").ToString().Contains("test-suite")) {
          XmlElement rootSuite = (XmlElement)node;
          ParseTestSuite(rootSuite, 0);
        }
      }

    }

    private void ParseTestSuite(XmlElement testSuite, int depth) {
      if (!testSuite.HasChildNodes) {
        return;
      }

      string line = new string(' ', 3*(depth-1));
      line += " - " + testSuite.Attributes.GetNamedItem("name").ToString() + ":";

      foreach (XmlElement node in testSuite.ChildNodes) {
        if(node.Name.Contains("test-case")) {
          ParseTestCase(node, depth+1);
        } else if (node.Name.Contains("test-suite")) {
          ParseTestSuite(node, depth+1);
        } 
      }
    }

    private void ParseTestCase(XmlElement testCase, int depth) {
      string line = new string(' ', 3*(depth-1));
      line += " - " + testCase.Attributes.GetNamedItem("name").ToString() + ": ";
      
      string result = testCase.Attributes.GetNamedItem("result").ToString().ToLower();
      if (result.Contains("passed")) {
        line += "✓";
        Console.WriteLine(line);
      } else {
        line += "✘";
        Console.WriteLine(line);
        XmlElement failure = (XmlElement)testCase.SelectSingleNode("failure");
        XmlElement message = (XmlElement)testCase.SelectSingleNode("message");
        XmlElement stack = (XmlElement)testCase.SelectSingleNode("stack-trace");
        Console.Write("\n\n");
        Console.WriteLine(message.InnerText);
        Console.WriteLine(stack.InnerText);
        Console.Write("\n\n");
      }
    }

    private string GetResultsPath() {
      string path = CLITools.GetArgument("testResults");
      if (string.IsNullOrEmpty(path)) {
        throw new Exception("No testing path specificied! Specify an XML output filepath using -testResults.");
      }

      return path;
    }

    private string RebuildArguments(string resultsPath) {
      string com = ""; // unity path
      foreach (string command in relevantCommands) {
        com += command + " ";
      }
      com += " -runTests";
      com += " -testResults {0}";
      return string.Format(com, resultsPath);
    }

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
        command.Contains("humanbuilders.cli.test") ||
        command.Contains("quit")  ||                  // This breaks testing for some reason.
        command.Contains("testResults")               // handled explicitly
      );
    }
  }
}

#endif