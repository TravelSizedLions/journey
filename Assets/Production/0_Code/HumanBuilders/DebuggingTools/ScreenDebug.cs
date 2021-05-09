using UnityEngine;
using UnityEngine.UI;

namespace HumanBuilders {

  public class ScreenDebug : MonoBehaviour {
    string myLog = "*begin log";
    string filename = "";
    int kChars = 700;

    /// <summary>
    /// The toggle for this script.
    /// </summary>
    [SerializeField]
    [Tooltip("The toggle for the script.")]
    private UnityEngine.UI.Toggle toggle = null; 

    private void Awake() {
      if (toggle != null) {
        if (!toggle.isOn) {
          enabled = false;
        }
      }
    }

    public void Toggle() {
      enabled = !enabled;
    }

    void OnEnable() { 
      Application.logMessageReceived += Log; 
    }


    void OnDisable() {
       Application.logMessageReceived -= Log;
    }
    public void Log(string logString, string stackTrace, LogType type) {
      if (enabled) {
        // for onscreen...
        myLog = myLog + "\n" + logString;
        if (myLog.Length > kChars) { myLog = myLog.Substring(myLog.Length - kChars); }
      }

      // for the file ...
      if (filename == "") {
        string d = System.Environment.GetFolderPath(
          System.Environment.SpecialFolder.Desktop) + "/YOUR_LOGS";
        System.IO.Directory.CreateDirectory(d);
        string r = Random.Range(1000, 9999).ToString();
        filename = d + "/log-" + r + ".txt";
      }

      try { 
        System.IO.File.AppendAllText(filename, logString + "\n"); 
      } catch { 

      }
    }

    void OnGUI() {
      if (!enabled) { return; }

      GUI.matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.identity,
        new Vector3(Screen.width / 1200.0f, Screen.height / 800.0f, 1.0f));
      GUI.TextArea(new Rect(10, 10, 540, 370), myLog);
    }
  }
}