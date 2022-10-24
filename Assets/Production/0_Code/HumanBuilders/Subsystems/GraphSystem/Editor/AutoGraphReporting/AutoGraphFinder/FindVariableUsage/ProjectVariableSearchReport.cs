using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders.Graphing.Editor {
  public class ProjectVariableSearchReport : ProjectReport<SceneVariableSearchReport> {
    public ProjectVariableSearchReport(string varName) : base(varName) {}
  }
}