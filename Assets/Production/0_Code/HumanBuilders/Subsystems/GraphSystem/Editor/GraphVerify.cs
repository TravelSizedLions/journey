#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using HumanBuilders;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using XNode;

namespace HumanBuilders.Editor {
  public static class GraphVerify {
    [MenuItem("AutoGraph/Analyze AutoGraphs In Scene")]
    public static void AnalyzeAutoGraphsInScene() {
      VerifyAutoGraphsInScene(out string message);
      Debug.Log(message);
    }

    public static bool VerifyAutoGraphsInScene(out string message) {
      Scene scene = SceneManager.GetActiveScene();
      List<AutoGraph> graphs = GetAutoGraphs(scene);
      List<GraphReport> reports = AnalyzeAllGraphs(graphs);
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

    public static List<GraphReport> AnalyzeAllGraphs(List<AutoGraph> graphs) {
      List<GraphReport> reports = new List<GraphReport>();
      foreach (AutoGraph graph in graphs) {
        reports.Add(AnalyzeGraph(graph));
      }
      return reports;
    }

    public static GraphReport AnalyzeGraph(AutoGraph graph) {
      GraphReport report = new GraphReport(graph);
      int attempts = 0;

      while (!report.FullyConnected && attempts < 3) {
        report.Analyze();
        attempts++;
      }

      return report;
    }

    public static string GetReports(List<GraphReport> reports, out bool allGood) {
      string message = "";
      foreach (GraphReport report in reports) {
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

  public class GraphReport {
    public AutoGraph Graph { get { return graph; } }
    public bool FullyConnected { get { return complete; } }
    public int Nodes { get { return totalNodes; } }
    public int IncompleteNodes { get { return totalIncompleteNodes; } }


    private AutoGraph graph;

    private string objectPath;

    private int totalNodes;

    private int totalIncompleteNodes;

    private bool complete;

    private List<NodeReport> nodeReports;

    public GraphReport(AutoGraph graph) {
      this.graph = graph;
    }

    public void Analyze() {
      BuildObjectPath();
      totalNodes = graph.Nodes.Count;
      totalIncompleteNodes = 0;

      nodeReports = new List<NodeReport>();
      if (graph.Nodes != null) {
        foreach (AutoNode node in graph.Nodes) {
          nodeReports.Add(AnalyzeNode(node));
        }
      }

      complete = (totalIncompleteNodes == 0);
    }

    private void BuildObjectPath() {
      string path = "";
      path = graph.name;
      Transform parent = graph.transform.parent;
      while (parent != null) {
        path = parent.name + " > " + path;
        parent = parent.parent;
      }

      objectPath = path;
    }

    private NodeReport AnalyzeNode(AutoNode node) {
      NodeReport report = new NodeReport(node);
      report.Analyze();
      totalIncompleteNodes += report.FullyConnected ? 0 : 1;
      return report;
    }

    public override string ToString() {
      string message = "";
      message += "Graph: " + objectPath + "\n";
      message += " - " + totalIncompleteNodes + "/" + totalNodes + " incomplete nodes:\n";
      foreach (NodeReport report in nodeReports) {
        if (!report.FullyConnected) {
          message += report.ToString() + "\n";
        }
      }
      return message;
    }

  }

  public class NodeReport {

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

    public NodeReport(AutoNode node) {
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

      complete = node.IsComplete();
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