#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Xml;
using System.Diagnostics;
using UnityEngine;
using UnityEditor.TestTools.TestRunner.Api;

namespace HumanBuilders {
  public class TestCommand : ICallbacks {

    //-------------------------------------------------------------------------
    // Constants
    //-------------------------------------------------------------------------
    private const string TEST_SUITE = "test-suite";
    private const string TEST_CASE = "test-case";
    private const string TEST_NAME = "name";
    private const string PASS_SYMBOL = "✓";
    private const string FAIL_SYMBOL = "✘";
    private const string SKIP_SYMBOL = "~";

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private string unityPath;
    private TestMode? testMode;
    private TestRunnerApi runner;
    private bool synchronous;
    private bool silent;
    private bool verbose;

    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------
    public TestCommand() {}

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void ParseCommandLine() {
      unityPath = CLITools.GetArgument(0);

      testMode = GetTestMode();
      synchronous = CLITools.GetFlag("runSynchronously") || CLITools.GetFlag("sync") || CLITools.GetFlag("synchronous");
      silent = CLITools.GetFlag("silent");
      verbose = !silent && (CLITools.GetFlag("verbose") || CLITools.GetFlag("v"));
      runner = ScriptableObject.CreateInstance<TestRunnerApi>();
    }

    public void Execute() {
      //  unity -batchmode -nographics -runTests -runSynchronously -projectPath
      //  . -logFile -testResults /j/logs/test.xml

      // unity -batchmode -nographics -executeCommand HumanBuilders.CLI.Test
      // -testResults /j/logs/test.xml

      Filter filter = CreateFilter(testMode);
      ExecutionSettings settings = new ExecutionSettings(filter);
      settings.runSynchronously = synchronous;
      runner.RegisterCallbacks(this);
      string result = runner.Execute(settings);
    }

    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    private TestMode? GetTestMode() {
      if (CLITools.GetFlag("playMode")) {
        return TestMode.PlayMode;
      } else if (CLITools.GetFlag("editMode")) {
        return TestMode.EditMode;
      } 

      return null;
    }

    private Filter CreateFilter(TestMode? mode) {
      if (mode != null) {
        return new Filter() {
          testMode = (TestMode)mode
        };
      }

      return new Filter();
    }


    public void RunStarted(ITestAdaptor testsToRun) {}

    public void RunFinished(ITestResultAdaptor results) {
      if (silent && results.FailCount == 0) {
        return;
      }

      if (verbose || results.FailCount > 0) {
        CLITools.PrintBanner("Tests");
        PrintTest(results, 0);
      }

      Console.WriteLine();
      CLITools.PrintInlineBanner("Test Summary");
      Console.WriteLine(" Passed: " + results.PassCount);
      Console.WriteLine(" Failed: " + results.FailCount);
      Console.WriteLine(" Skipped/Inconclusive: " + (results.SkipCount + results.InconclusiveCount));
      CLITools.PrintBannerBar();
      Console.WriteLine();
    }

    public void TestStarted(ITestAdaptor test) {}

    public void TestFinished(ITestResultAdaptor result) {}

    private void PrintTest(ITestResultAdaptor test, int depth) {
      if (test.HasChildren) {
        string message = "";
        if (depth != 0) {
          message = new string(' ', 3*(depth-1));
          message += " - ";
        }

        switch (test.TestStatus) {
          case TestStatus.Passed:
            message += string.Format("[{0}] ", PASS_SYMBOL);
            break;
          case TestStatus.Failed:
            message += string.Format("[{0}] ", FAIL_SYMBOL);
            break;
          default:
            message += string.Format("[{0}] ", SKIP_SYMBOL);
            break;
        }

        message += test.Name + " - " + test.Duration + "s";
        Console.WriteLine(message);

        foreach (ITestResultAdaptor childTest in test.Children) {
          PrintTest(childTest, depth+1);
        }
      } else {
        string message = "";
        if (depth != 0) {
          message = new string(' ', 3*(depth-1));
          message += " - ";
        }
        
        switch (test.TestStatus) {
          case TestStatus.Passed:
            message += string.Format("[{0}] ", PASS_SYMBOL);
            message += test.Name + " - " + test.Duration + "s";
            break;
          case TestStatus.Failed:
            message += string.Format("[{0}] ", FAIL_SYMBOL);
            message += test.Name;
            if (!string.IsNullOrEmpty(test.Message)) {
              message += "\n\n";
              message += test.Message;
            }
            message += "\n\n";
            message += test.StackTrace;
            break;
          default:
            message += string.Format("[{0}] ", SKIP_SYMBOL);
            message += test.Name + "...";
            break;
        }

        Console.WriteLine(message);
      }
    }
  }
}

#endif