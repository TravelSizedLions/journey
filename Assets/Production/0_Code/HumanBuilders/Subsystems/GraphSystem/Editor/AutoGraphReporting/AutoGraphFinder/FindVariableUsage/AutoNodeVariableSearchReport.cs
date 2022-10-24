using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HumanBuilders.Graphing.Editor {
  public class AutoNodeVariableSearchReport : AutoNodeReport {
    public string VariableName { get => variableName; }
    private string variableName;
    public bool ReferencesVariable { get => referencesVariable; }
    private bool referencesVariable;

    public AutoNodeVariableSearchReport(IAutoNode n, params object[] extraParams) : base(n) {
      variableName = (string)extraParams[0];

      // Here lies my integrity as a software architect. R.I.P.
      string message = node.GetType().ToString() + "\n\n";
      foreach (var field in node.GetType().GetFields()) {
        var value = field.GetValue(node);
        message += string.Format("{0} = {1}\n", field, field.GetValue(n));
        
        if (value != null) {
          var valueType = value.GetType();
          bool isList = (valueType.IsGenericType && (valueType.GetGenericTypeDefinition() == typeof(List<>)));
          if (isList) {
            foreach (var item in (IEnumerable)value) {
              var itemType = item.GetType();
              message += string.Format("   - {0}\n", item);
              foreach (var itemField in itemType.GetFields()) {
                var itemFieldVal = itemField.GetValue(item);
                message += string.Format("       - {0} = {1}\n", itemField, itemFieldVal);
              }
            }
          } else {          
            foreach (var subfield in value.GetType().GetFields()) {
              var subvalue = subfield.GetValue(value);
              message += string.Format("   - {0} = {1}\n", subfield, subvalue);
            }
          }
        }
      }
      
      referencesVariable = message.Contains(variableName);
    }

    protected override string BuildMessage() {
      if (referencesVariable) {
        return node.NodeName;
      }

      return "";
    }
  }

}