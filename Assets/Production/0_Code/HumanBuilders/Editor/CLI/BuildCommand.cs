#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;

namespace HumanBuilders {
  public class BuildCommand {
    //-------------------------------------------------------------------------
    // Constants
    //-------------------------------------------------------------------------
    private const string KEYSTORE_PASS = "KEYSTORE_PASS";
    private const string KEY_ALIAS_PASS = "KEY_ALIAS_PASS";
    private const string KEY_ALIAS_NAME = "KEY_ALIAS_NAME";
    private const string KEYSTORE = "keystore.keystore";
    private const string ANDROID_BUNDLE_VERSION_CODE = "BUNDLE_VERSION_CODE";
    private const string ANDROID_APP_BUNDLE = "BUILD_APP_BUNDLE";
    private const string SCRIPTING_BACKEND_ENV_VAR = "SCRIPTING_BACKEND";

    //-------------------------------------------------------------------------
    // Fields
    //-------------------------------------------------------------------------
    private string targetName;
    private BuildTarget target;
    private string buildName;
    private string buildPath;
    private string fixedBuildPath;
    private BuildOptions buildOpts;

    //-------------------------------------------------------------------------
    // Constructors
    //-------------------------------------------------------------------------
    public BuildCommand() {}

    //-------------------------------------------------------------------------
    // Public Interface
    //-------------------------------------------------------------------------
    public void ParseCommandLine() {
      target = GetBuildTarget();
      buildPath = GetBuildPath();
      buildName = GetBuildName();
      fixedBuildPath = GetFixedBuildPath(target, buildPath, buildName);
      buildOpts = GetBuildOptions();
    }

    public void Execute() {
      CLITools.PrintBanner("Build");

      if (target == BuildTarget.Android) {
        HandleAndroidAppBundle();
        HandleAndroidBundleVersionCode();
        HandleAndroidKeystore();
      }

      SetScriptingBackendFromEnv(target);

      string[] scenes = CLITools.GetEnabledScenes();
      BuildReport report = BuildPipeline.BuildPlayer(scenes, fixedBuildPath, target, buildOpts);

      if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded) {
        throw new Exception($"Build ended with {report.summary.result} status");  
      } else {
        PrintReport(report);
      }
    }

    //-------------------------------------------------------------------------
    // Helper Methods
    //-------------------------------------------------------------------------
    private void PrintReport(BuildReport report) {
      CLITools.PrintInlineBanner("Steps");

      foreach (BuildStep step in report.steps) {
        int depth = step.depth;
        string message = "";
        if (depth == 0) {
          message += "Step: ";
        } else {
          message += new String(' ', (depth-1)*3);
          message += " - ";
        }
        message += step.name + " - " + step.duration.TotalMilliseconds + "ms";
        Console.WriteLine(message);
      }

      CLITools.PrintInlineBanner("Build Complete");
    }

    private BuildTarget GetBuildTarget() {
      targetName = CLITools.GetArgument("target");
      if (targetName.TryConvertToEnum(out BuildTarget target)) {
        return target;
      }

      return BuildTarget.NoTarget;
    }

    private string GetBuildPath() {
      string buildPath = CLITools.GetArgument("dir");
      if (buildPath == "") {
        throw new Exception("dir argument is missing");
      }
      return buildPath;
    }

    private string GetBuildName() {
      string buildName = CLITools.GetArgument("name");
      Console.WriteLine(":: Received name " + buildName);
      if (buildName == "") {
        throw new Exception("name argument is missing");
      }
      return buildName;
    }

    private string GetFixedBuildPath(BuildTarget targ, string path, string name) {
      if (targ.ToString().ToLower().Contains("windows")) {
        name += ".exe";
      } else if (targ == BuildTarget.Android) {
        name += EditorUserBuildSettings.buildAppBundle ? ".aab" : ".apk";
      }
      return Path.Combine(path, buildName);
    } 

   private BuildOptions GetBuildOptions() {
      string options = CLITools.GetArgument("opts");
      string[] allOptionVars = options.Split(',');
      BuildOptions allOptions = BuildOptions.None;
      BuildOptions option;
      string optionVar;
      int length = allOptionVars.Length;

      for (int i = 0; i < length; i++) {
        optionVar = allOptionVars[i];

        if (optionVar.TryConvertToEnum(out option)) {
          allOptions |= option;
        } else {
          Console.WriteLine($":: Cannot convert {optionVar} to {nameof(BuildOptions)} enum, skipping it.");
        }
      }

      return allOptions;
    }

    private void SetScriptingBackendFromEnv(BuildTarget platform) {
      var targetGroup = BuildPipeline.GetBuildTargetGroup(platform);
      if (CLITools.TryGetEnv(SCRIPTING_BACKEND_ENV_VAR, out string scriptingBackend)) {
        if (scriptingBackend.TryConvertToEnum(out ScriptingImplementation backend)) {
          Console.WriteLine($":: Setting ScriptingBackend to {backend}");
          PlayerSettings.SetScriptingBackend(targetGroup, backend);
        } else {
          string possibleValues = string.Join(", ", Enum.GetValues(typeof(ScriptingImplementation)).Cast<ScriptingImplementation>());
          throw new Exception($"Could not find '{scriptingBackend}' in ScriptingImplementation enum. Possible values are: {possibleValues}");
        }
      } else {
        var defaultBackend = PlayerSettings.GetDefaultScriptingBackend(targetGroup);
        Console.WriteLine($":: Using project's configured ScriptingBackend (should be {defaultBackend} for tagetGroup {targetGroup}");
      }
    }

    //-------------------------------------------------------------------------
    // Android Stuff
    //-------------------------------------------------------------------------
    private static void HandleAndroidAppBundle() {
      if (CLITools.TryGetEnv(ANDROID_APP_BUNDLE, out string value)) {
        if (bool.TryParse(value, out bool buildAppBundle)) {
          EditorUserBuildSettings.buildAppBundle = buildAppBundle;
          Console.WriteLine($":: {ANDROID_APP_BUNDLE} env var detected, set buildAppBundle to {value}.");
        } else {
          Console.WriteLine($":: {ANDROID_APP_BUNDLE} env var detected but the value \"{value}\" is not a boolean.");
        }
      }
    }

    private static void HandleAndroidBundleVersionCode() {
      if (CLITools.TryGetEnv(ANDROID_BUNDLE_VERSION_CODE, out string value)) {
        if (int.TryParse(value, out int version)) {
          PlayerSettings.Android.bundleVersionCode = version;
          Console.WriteLine($":: {ANDROID_BUNDLE_VERSION_CODE} env var detected, set the bundle version code to {value}.");
        } else
          Console.WriteLine($":: {ANDROID_BUNDLE_VERSION_CODE} env var detected but the version value \"{value}\" is not an integer.");
      }
    }

    private static void HandleAndroidKeystore() {
      PlayerSettings.Android.useCustomKeystore = false;

      if (!File.Exists(KEYSTORE)) {
        Console.WriteLine($":: {KEYSTORE} not found, skipping setup, using Unity's default keystore");
        return;
      }

      PlayerSettings.Android.keystoreName = KEYSTORE;

      string keystorePass;
      string keystoreAliasPass;

      if (CLITools.TryGetEnv(KEY_ALIAS_NAME, out string keyaliasName)) {
        PlayerSettings.Android.keyaliasName = keyaliasName;
        Console.WriteLine($":: using ${KEY_ALIAS_NAME} env var on PlayerSettings");
      } else {
        Console.WriteLine($":: ${KEY_ALIAS_NAME} env var not set, using Project's PlayerSettings");
      }

      if (!CLITools.TryGetEnv(KEYSTORE_PASS, out keystorePass)) {
        Console.WriteLine($":: ${KEYSTORE_PASS} env var not set, skipping setup, using Unity's default keystore");
        return;
      }

      if (!CLITools.TryGetEnv(KEY_ALIAS_PASS, out keystoreAliasPass)) {
        Console.WriteLine($":: ${KEY_ALIAS_PASS} env var not set, skipping setup, using Unity's default keystore");
        return;
      }

      PlayerSettings.Android.useCustomKeystore = true;
      PlayerSettings.Android.keystorePass = keystorePass;
      PlayerSettings.Android.keyaliasPass = keystoreAliasPass;
    }
  }
}
#endif
