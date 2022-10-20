#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using HumanBuilders;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using XNode;
using UnityEditor.SceneManagement;

namespace HumanBuilders.Editor {
  public static class SearchForVariableUsages {
    [MenuItem("Journey/Search For Variable Usages")]
    public static void Search() {
      foreach (var scenePath in GetAllScenesInBuild()) {
        string fileName = scenePath.Split('/')[scenePath.Split('/').Length-1].Split('.')[0];
        Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Single);
        bool isGood = GraphVariableSearch.VerifyAutoGraphsInScene(out string message);
        if (!isGood) {
          message = "----- " + fileName + " -----\n" + message;
          Debug.Log(message);
        }
      }

      /**
        For each scene:
          - open the scene
          - search for variables in autographs on:
            - ShowHideCompanionNode
            - ToggleCompanionFollowNode
            - RetrieveValueNode
            - StoreValueNode
            - less fragile way of doing this...? Probably use reflection to get
              types of fields/properties on classes so you don't have to check
              for every specific class

          - search entire scene for objects with references to variables on:
            - VConditions
            - ConditionListeners
            - MultiConditionListeners
            - ObjectActiveListeners
            - VTriggers
            - more robust way: similar to above...?

        render/log list of items.
      */
    }

    
    public static List<string> GetAllScenesInBuild() {
      List<string> scenes = new List<string>();
      
      int sceneCount = SceneManager.sceneCountInBuildSettings;
      for (int i = 0; i < sceneCount; i++) {
        string path = SceneUtility.GetScenePathByBuildIndex(i);
        if (!string.IsNullOrEmpty(path)) {
          scenes.Add(path);
        }
      }

      return scenes;
    }
  }

  public static class GraphVariableSearch {
    public static void AnalyzeAutoGraphsInScene() {
      VerifyAutoGraphsInScene(out string message);
      Debug.Log(message);
    }

    public static bool VerifyAutoGraphsInScene(out string message) {
      Scene scene = SceneManager.GetActiveScene();
      List<AutoGraph> graphs = GetAutoGraphs(scene);
      List<GraphReport2> reports = AnalyzeAllGraphs(graphs);
      message = GetReports(reports, out bool allGood);

      return allGood;
    }

    public static List<AutoGraph> GetAutoGraphs(Scene scene) {
      List<AutoGraph> graphs = new List<AutoGraph>();

      foreach (GameObject obj in scene.GetRootGameObjects())  {
        graphs.AddRange(obj.GetComponentsInChildren<AutoGraph>(true));
      }

      return graphs;
    }

    public static List<GraphReport2> AnalyzeAllGraphs(List<AutoGraph> graphs) {
      List<GraphReport2> reports = new List<GraphReport2>();
      foreach (AutoGraph graph in graphs) {
        reports.Add(AnalyzeGraph(graph));
      }
      return reports;
    }

    public static GraphReport2 AnalyzeGraph(IAutoGraph graph) {
      GraphReport2 report = new GraphReport2(graph);
      int attempts = 0;

      while (!report.FullyConnected && attempts < 3) {
        report.Analyze();
        attempts++;
      }

      return report;
    }

    public static string GetReports(List<GraphReport2> reports, out bool allGood) {
      string message = "";
      foreach (GraphReport2 report in reports) {
        if (!report.FullyConnected) {
          if (string.IsNullOrEmpty(message)) {
            message = "Click to see report of incomplete graphs...\n\n";
          }
          message += report.ToString() + "\n";
        }
      }

      if (string.IsNullOrEmpty(message)) {
        allGood = true;
        return "All graphs complete!";
      } else {
        allGood = false;
        return message;
      }
    }
  }

  public class GraphReport2 {
    public IAutoGraph Graph { get { return graph; } }
    public bool FullyConnected { get { return complete; } }
    public int Nodes { get { return totalNodes; } }
    public int IncompleteNodes { get { return totalIncompleteNodes; } }


    private IAutoGraph graph;

    private string objectPath;

    private int totalNodes;

    private int totalIncompleteNodes;

    private bool complete;

    private List<NodeReport2> nodeReports;

    public GraphReport2(IAutoGraph graph) {
      this.graph = graph;
    }

    public void Analyze() {
      BuildObjectPath();
      totalNodes = graph.AutoNodes.Count;
      totalIncompleteNodes = 0;

      nodeReports = new List<NodeReport2>();
      if (graph.AutoNodes != null) {
        foreach (AutoNode node in graph.AutoNodes) {
          nodeReports.Add(AnalyzeNode(node));
        }
      }

      complete = (totalIncompleteNodes == 0);
    }

    private void BuildObjectPath() {
      string path = "";
      path = graph.GraphName;
      if (typeof(AutoGraphAsset).IsAssignableFrom(graph.GetType())) {
        path = AssetDatabase.GetAssetPath((AutoGraphAsset)graph);
      } else if (typeof(AutoGraph).IsAssignableFrom(graph.GetType())) {
        Transform parent = ((AutoGraph)graph).transform.parent;
        while (parent != null) {
          path = parent.name + " > " + path;
          parent = parent.parent;
        }
      }

      objectPath = path;
    }

    private NodeReport2 AnalyzeNode(AutoNode node) {
      NodeReport2 report = new NodeReport2(node);
      report.Analyze();
      totalIncompleteNodes += report.FullyConnected ? 0 : 1;
      return report;
    }

    public override string ToString() {
      string message = "";
      message += "Graph: " + objectPath + "\n";
      message += " - " + totalIncompleteNodes + "/" + totalNodes + " nodes incomplete:\n";
      foreach (NodeReport2 report in nodeReports) {
        if (!report.FullyConnected) {
          message += report.ToString() + "\n";
        }
      }
      return message;
    }

  }

  public class NodeReport2 {

    public bool FullyConnected { get { return complete; } }
    public int Ports { get { return totalPorts; } }
    public int OutputPorts { get {return totalOutputPorts; } }
    public int InputPorts { get { return totalInputPorts; } }
    public int UnconnectedPorts { get { return totalUnconnectedPorts; } }

    public AutoNode Node { get {return node; } }

    private AutoNode node;
    private int totalPorts;
    private int totalOutputPorts;
    private int totalInputPorts;
    private bool complete;
    private int totalUnconnectedPorts;

    public NodeReport2(AutoNode node) {
      this.node = node;
    }

    public void Analyze() {
      totalPorts = 0;
      totalInputPorts = 0;
      totalOutputPorts = 0;
      foreach (NodePort port in node.Ports) {
        totalPorts++;
        totalOutputPorts += port.IsOutput ? 1 : 0;
        totalInputPorts += port.IsInput ? 1 : 0;
      }

      complete = node.IsNodeComplete();
      totalUnconnectedPorts = complete ? 0 : node.TotalDisconnectedPorts();
    }

    public override string ToString() {
      string message = "";
      message += "   - " + node.GetType().FullName.Split('.')[node.GetType().FullName.Split('.').Length-1] + ": ";
      message += totalUnconnectedPorts + " unconnected ports";
      // message += "      - Total Input: " + totalInputPorts + ", ";
      // message += "Total Output: " + totalOutputPorts + "\n";
      // message += "      - Unconnected Ports: "  + totalUnconnectedPorts;
      return message;
    }
  }
}

#endif