#if UNITY_EDITOR
using System;
using System.Linq;
using UnityEditor;

namespace HumanBuilders {
  public static class CLITools {
    /// <summary>
    /// Print out an 80x3 banner with the given title
    /// </summary>
    /// <param name="title">The title of the banner</param>
    public static void PrintBanner(string title) {
      string bannerBar = new string('-', 80);
      title = " " + title;
      Console.WriteLine(bannerBar);
      Console.WriteLine(title);
      Console.WriteLine(bannerBar);
    }

    public static void PrintInlineBanner(string title) {
      string banner = new string('-', 4);
      banner += " ";
      banner += title;
      banner += " ";
      banner += new string('-', 80 - banner.Length);
      Console.WriteLine(banner);
    }

    public static void PrintBannerBar() {
      Console.WriteLine(new string('-', 80));
    }

    /// <summary>
    /// Get the value of a command line argument.
    /// </summary>
    /// <param name="name">The name of the argument to get.</param>
    /// <returns>The value of the argument</returns>
    public static string GetArgument(string name) {
      string[] args = Environment.GetCommandLineArgs();
      for (int i = 0; i < args.Length; i++) {
        if (args[i].Substring(1) == name) {
          return args[i + 1];
        }
      }
      return null;
    }

    public static bool GetFlag(string name) {
      string[] args = Environment.GetCommandLineArgs();
      for (int i = 0; i < args.Length; i++) {
        if (args[i].Substring(1) == name) {
          return true;
        }
      }

      return false;
    }

    public static string GetArgument(int index) {
      return Environment.GetCommandLineArgs()[index];
    }

    public static string[] GetEnabledScenes() {
      return (
        from scene in EditorBuildSettings.scenes 
          where scene.enabled 
          where!string.IsNullOrEmpty(scene.path) 
          select scene.path
      ).ToArray();
    }


    public static bool TryConvertToEnum<TEnum>(this string strEnumValue, out TEnum value) {
      if (!Enum.IsDefined(typeof(TEnum), strEnumValue)) {
        value = default;
        return false;
      }

      value = (TEnum) Enum.Parse(typeof(TEnum), strEnumValue);
      return true;
    }

    public static bool TryGetEnv(string key, out string value) {
      value = Environment.GetEnvironmentVariable(key);
      return !string.IsNullOrEmpty(value);
    }
  }
}
#endif
