#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders {
  public class ConditionTableAttributeDrawer : OdinAttributeDrawer<ConditionTableAttribute, List<ConditionTableEntry>> {
    private LocalPersistentContext<bool> expanded;

    protected override void Initialize() {
      expanded = this.GetPersistentValue<bool>(
        "ConditionTableAttributeDrawer.expanded",
        this.Attribute.StartExpanded
      );
    }
    
    protected override void DrawPropertyLayout(GUIContent label) {
      GUIHelper.PushColor(new Color(this.Attribute.R, this.Attribute.G, this.Attribute.B, this.Attribute.A), true);
      SirenixEditorGUI.BeginBox();
      SirenixEditorGUI.BeginBoxHeader();
      GUIHelper.PopColor();
      List<ConditionTableEntry> entries = new List<ConditionTableEntry>();

      SirenixEditorGUI.Title(this.Attribute.Title, "", TextAlignment.Center, false, true);

      if (SirenixEditorGUI.IconButton(EditorIcons.Plus)) {
        entries.Add(new ConditionTableEntry());
      }

      SirenixEditorGUI.EndBoxHeader();

      for (int i = 0; i < this.Property.Children.Count; i++) {
        SirenixEditorGUI.BeginBox();
        SirenixEditorGUI.BeginIndentedHorizontal(new GUILayoutOption[] {});
        bool keep = !SirenixEditorGUI.IconButton(EditorIcons.Minus);

        if (keep) {
          SirenixEditorGUI.Title("Condition "+i+" ", "", TextAlignment.Right, false);
          GUILayoutOption[] options = { GUILayout.MinWidth(245) };
          ICondition condition = (ICondition)SirenixEditorFields.UnityObjectField(
            "",
            (Object)this.ValueEntry.SmartValue[i].Condition,
            typeof(ICondition),
            false,
            options
          );

          this.ValueEntry.SmartValue[i].Condition = condition;

          entries.Add(
            this.ValueEntry.SmartValue[i]
          );
        }
        
        SirenixEditorGUI.EndIndentedHorizontal();
        SirenixEditorGUI.EndBox();
      }

      this.ValueEntry.SmartValue = entries;

      if (entries.Count == 0) {
        SirenixEditorGUI.MessageBox("Press \"+\" to add extra conditions.", MessageType.None, true);
      }
      SirenixEditorGUI.EndBox();
    }
  }
}
#endif
