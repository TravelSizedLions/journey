#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace HumanBuilders {
  public static class CLI {
    public static void Build() {
      BuildCommand command = new BuildCommand();
      command.ParseCommandLine();
      command.Execute();
    }

    public static void Test() {
      TestCommand command = new TestCommand();
      command.ParseCommandLine();
      command.Execute();
    }
  }
}

#endif
