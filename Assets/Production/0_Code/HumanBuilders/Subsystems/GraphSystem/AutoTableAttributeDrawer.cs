#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace HumanBuilders {
  // [OdinDrawer]
  // [DrawerPriority(DrawerPriorityLevel.ValuePriority)]
  public class AutoTableAttributeDrawer<TList, TElement> : OdinAttributeDrawer<AutoTableAttribute, TList>
    where TList : List<TElement> 
    where TElement : new() {

    private LocalPersistentContext<bool> expanded;

    protected override void Initialize() {
      expanded = this.GetPersistentValue<bool>("AutoTableAttributeDrawer.expanded", this.Attribute.expanded);
    }


    protected override void DrawPropertyLayout(GUIContent label) {
      SirenixEditorGUI.BeginBox();

      bool collectionChanged = false;
      MakeHeader(ref collectionChanged);

      if (collectionChanged) {
        expanded.Value = true;
      }

      if (SirenixEditorGUI.BeginFadeGroup(this, expanded.Value)) {
        if (!DrawEntries(ref collectionChanged)) {
          DrawPrompt();
        }

        SirenixEditorGUI.EndFadeGroup();
      }


      if (collectionChanged) {
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
      }

      SirenixEditorGUI.EndBox();
    }

    private void MakeHeader(ref bool collectionChanged) {
      if (ColorUtility.TryParseHtmlString(this.Attribute.ColorHex, out Color color)) {
        GUIHelper.PushColor(color, true);
      } else {
        GUIHelper.PushColor(Color.white, true);
      }

      SirenixEditorGUI.BeginBoxHeader();
      GUIHelper.PopColor();

      string title = string.IsNullOrEmpty(this.Attribute.Title) ? this.Property.NiceName : this.Attribute.Title;
      title = "[" + this.ValueEntry.SmartValue.Count + "] " + title;

      expanded.Value = SirenixEditorGUI.Foldout(expanded.Value, title);

      if (SirenixEditorGUI.IconButton(EditorIcons.Plus)) {
        this.ValueEntry.SmartValue.Add(new TElement());
        collectionChanged = true;
      }

      SirenixEditorGUI.EndBoxHeader();
    }


    private bool DrawEntries(ref bool collectionChanged) {
      // While loop used since it's possible to modify the list mid iteration.
      int index = 0;
      while (index < this.Property.Children.Count) {
        index = DrawEntry(index, ref collectionChanged);
      }

      return this.ValueEntry.SmartValue.Count > 0;
    }

    private int DrawEntry(int index, ref bool collectionChanged) {
      SirenixEditorGUI.BeginIndentedHorizontal(new GUILayoutOption[] {});
      SirenixEditorGUI.BeginBox();
      SirenixEditorGUI.Title(this.Attribute.ListItemType.Name.Split('`')[0] + " " + index + "", "", TextAlignment.Center, true, false);
      SirenixEditorGUI.BeginIndentedVertical();

      EditorGUI.BeginChangeCheck();

      // Don't draw the crappy header. Just skip to the actual type properties.
      foreach (var child in this.Property.Children[index].Children) {
        child.Draw();
      }

      if (EditorGUI.EndChangeCheck()) {
        collectionChanged = true;
      }


      SirenixEditorGUI.EndIndentedVertical();
      SirenixEditorGUI.EndBox();
      if (!SirenixEditorGUI.IconButton(EditorIcons.Minus)) {
        index++;
      } else {
        this.ValueEntry.SmartValue.RemoveAt(index);
      }
      SirenixEditorGUI.EndIndentedHorizontal();

      return index;
    }

    public void DrawPrompt() {
      SirenixEditorGUI.MessageBox("Press \"+\" to add a new " + this.Attribute.ListItemType.Name.Split('`')[0] + ".", MessageType.None, true);
    }
  }
}
#endif
