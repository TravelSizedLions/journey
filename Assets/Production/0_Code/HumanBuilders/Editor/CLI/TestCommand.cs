#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace HumanBuilders {
  public class TestCommand {
    //-------------------------------------------------------------------------
    // Constants
    //-------------------------------------------------------------------------

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private string unityPath;
    private string testResultsPath;

    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------
    public TestCommand() {}

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void ParseCommandLine() {
      unityPath = CLITools.GetArgument(0);
      testResultsPath = CLITools.GetArgument("testResults");

      // string command = "{0}";
    }

    public void Execute() {
      //  unity -batchmode -nographics -runTests -runSynchronously -projectPath
      //  . -logFile -testResults /j/logs/test.xml

      // unity -batchmode -nographics -executeCommand HumanBuilders.CLI.Test
      // -testResults /j/logs/test.xml
      
      ShowResults();
    }

    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------

    private void ShowResults() {
      XmlDocument doc = new XmlDocument();
      doc.Load(testResultsPath);

      foreach (XmlNode node in doc.ChildNodes) {
        if (node.Attributes.GetNamedItem("name").ToString().Contains("test-suite")) {
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