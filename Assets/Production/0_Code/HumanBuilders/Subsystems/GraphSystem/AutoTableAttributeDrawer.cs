#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders {
  public class AutoTableAttributeDrawer<TList, TElement> : OdinAttributeDrawer<AutoTableAttribute, TList>
    where TList : List<TElement> 
    where TElement : UnityEngine.Object {

    protected override void DrawPropertyLayout(GUIContent label) {

      TElement before = null;
      if (this.ValueEntry.SmartValue.Count == 1) {
        before = this.ValueEntry.SmartValue[0];
      }

      SirenixEditorGUI.BeginBox();

      bool collectionChanged = false;

      MakeHeader(ref collectionChanged);

      if (!DrawEntries(ref collectionChanged)) {
        DrawPrompt();
      }

      if (collectionChanged) {
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
      }

      if (this.ValueEntry.SmartValue.Count == 1) {
        if (before != this.ValueEntry.SmartValue[0]) {
          // Debug.Log("changed");

        }
      }

      SirenixEditorGUI.EndBox();
    }

    public void MakeHeader(ref bool collectionChanged) {
      if (ColorUtility.TryParseHtmlString(this.Attribute.ColorHex, out Color color)) {
        GUIHelper.PushColor(color, true);
      } else {
        GUIHelper.PushColor(Color.white, true);
      }

      SirenixEditorGUI.BeginBoxHeader();
      GUIHelper.PopColor();

      string title = string.IsNullOrEmpty(this.Attribute.Title) ? this.Property.NiceName : this.Attribute.Title; 
      SirenixEditorGUI.Title(title, "", TextAlignment.Left, false, false);

      if (SirenixEditorGUI.IconButton(EditorIcons.Plus)) {
        this.ValueEntry.SmartValue.Add(null);
      }

      SirenixEditorGUI.EndBoxHeader();
    }


    public bool DrawEntries(ref bool collectionChanged) {
      // AssetDatabase.Refresh();

      // While loop used since it's possible to modify the list mid iteration.
      int index = 0;
      while (index < this.ValueEntry.SmartValue.Count) {
        index = DrawEntry(index, ref collectionChanged);
      }

      return this.ValueEntry.SmartValue.Count > 0;
    }

    public int DrawEntry(int index, ref bool collectionChanged) {
      SirenixEditorGUI.BeginBox();
      SirenixEditorGUI.BeginIndentedHorizontal(new GUILayoutOption[] {});
      bool keep = !SirenixEditorGUI.IconButton(EditorIcons.Minus);

      if (keep) {
        SirenixEditorGUI.Title(this.Attribute.ListItemType.Name.Split('`')[0] + " " + index + ": ", "", TextAlignment.Right, false, false);
        GUILayoutOption[] options = { GUILayout.MinWidth(245) };
  
        TElement before = this.ValueEntry.SmartValue[index];
        this.ValueEntry.SmartValue[index] = (TElement)SirenixEditorFields.UnityObjectField(
          "",
          this.ValueEntry.SmartValue[index],
          this.Attribute.ListItemType,
          false
        );

        collectionChanged = collectionChanged || (before != this.ValueEntry.SmartValue[index]);

        index++;
      } else {
        this.ValueEntry.SmartValue.RemoveAt(index);
      }
      
      SirenixEditorGUI.EndIndentedHorizontal();
      SirenixEditorGUI.EndBox();

      return index;
    }

    public void DrawPrompt() {
      SirenixEditorGUI.MessageBox("Press \"+\" to add a new " + this.Attribute.ListItemType.Name.Split('`')[0] + ".", MessageType.None, true);
    }
  }
}
#endif
