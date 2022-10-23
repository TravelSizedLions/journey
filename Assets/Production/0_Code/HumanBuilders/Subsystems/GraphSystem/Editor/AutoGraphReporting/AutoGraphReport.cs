#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders.Editor {
  public abstract class AutoGraphReport<T>: Report where T: AutoNodeReport  {

    public List<T> NodeReports { get => nodeReports; }
    protected List<T> nodeReports;

    public IAutoGraph Graph { get => graph; }
    protected IAutoGraph graph;

    public AutoGraphReport(IAutoGraph g) {
      graph = g;
      nodeReports = TraverseGraph(g);
    }

    private List<T> TraverseGraph(IAutoGraph graph) {
      List<T> analyses = new List<T>();
      graph.AutoNodes.ForEach((IAutoNode node) => analyses.Add((T)Activator.CreateInstance(typeof(T), node)));
      return analyses;
    }

    
    protected static string GetHierarchyPathToGraph(IAutoGraph graph) {
      string path = "missing!";
      path = graph?.GraphName;

      if (typeof(AutoGraphAsset).IsAssignableFrom(graph.GetType())) {
        path = AssetDatabase.GetAssetPath((AutoGraphAsset)graph);
      } else if (typeof(AutoGraph).IsAssignableFrom(graph.GetType())) {
        Transform parent = ((AutoGraph)graph).transform.parent;
        while (parent != null) {
          path = parent.name + " > " + path;
          parent = parent.parent;
        }
      }

      return path;
    }
  }
}
#endif