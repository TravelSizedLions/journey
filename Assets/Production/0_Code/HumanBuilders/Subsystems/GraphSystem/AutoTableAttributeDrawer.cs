#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using XNodeEditor;

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

      }
      
      SirenixEditorGUI.EndFadeGroup();


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

      foreach (var child in this.Property.Children[index].Children) {
        child.Draw();
        if (true /*is node port?*/) {
          NodeEditorGUILayout.PortField(/*port*/ null, new GUILayoutOption[] {});
          /*
          To draw a port field here:
            - have a port to populate this field
            - the port should represent the set of properties for this item in
              the list
            - I (hopefully) shouldn't have to add a port within the dynamic list
              property's type
          To get the ports:
            - The list of ports is stored in the node at edit time. Somehow,
              they need to end up here, and in the right order.
            - The NodeEditor does something similar already using attributes, so
              it can't be *totally* impossible.
          Complications:
            - This property drawer isn't specific to the Node Editor Window.
              This means ports shouldn't get drawn in every context.
            - There doesn't seem to be a way to get info about the node directly
              into the property attribute as would be the simplest solution...
              instead, online the suggestion is to use some kind of 3rd class to
              look at class instances and pull information from point A to point
              B. Sounds difficult to do elegantly in this scenario...but again,
              the NodeEditor can accomplish drawing port handles over existing 
              property draws, so it must be possible.
            Observations:
              - NodeEditor passes target.DynamicPorts into
                NodeEditorGUILayout.PortField. That's the call we care about.
              - target is almost certainly the Node that the NodeEditor is
                tasked with rendering.
              - target is defined in NodeEditorBase, as generic type K (which
                for us is XNode.Node...)
              - target seems to be handed to NodeEditorBase via GetEditor, which
                seems like some weird roundabout constructor...
                - is a static method that creates an instance of type T (NodeEditor in our case,
                  which again directly inherits from NodeEditorBase) using
                  reflection
                - populates target (and other props)
                - hands back the created instance
              - NodeEditorBase.GetEditor() used by:
                - NodeEditor: to add context menu stuff (not useful)
                - NodeEditorAction: to handle mouse behavior (not useful)
                - NodeEditorGUI: to draw all the nodes (sounds about right)
                  - Soon after GetEditor(), NodeEditorGUI calls OnBodyGUI() on
                    the instance, which in turn draws the port fields, so yes,
                    this is correct spot...
                  - NodeEditorGUI gets the node in question from a reference to
                    the graph.
                  - graph in this case is a property belonging to
                    NodeEditorGUI's outer class...yes, NodeEditorGUI is
                    enclosed within a partial class of NodeEditorWindow...wow,
                    the code smell is funky down here in XNode land.
                  - this, in turn, looks to be set in NodeEditorWindow.Open()
                - NodeEditorWindow:
                  - Open() called by OnOpen(), which pulls in the graph via
                    Unity API (EditorUtility.InstanceIDToObject()), which is fed
                    an instanceID given to them by (you guessed it) the Unity
                    Engine...crap. That's gonna be way too much work.
              - Where do I already have access to the graph?
                - well, the graph has access to itself (obviously)
                - each node has a reference to the graph.
                - I can create a custom node editor script, and that would have
                  access to the graph.
                - Right now, my assumption is that NodeEditors are specific to a
                  given type of node. I could supply a NodeEditor that's
                  specific to nodes of type AutoNode...but, well, much of the
                  drawing logic would be identical.
                  - NodeEditor walks through all visible properties and draws
                    them using NodeEditorGUILayout.PropertyField.
                  - Wait, the call we care about may actually be PropertyField,
                    which supposedly automatically displays ports if relevant. How
                    the heck does that work?
                  - It looks like that method grabs the Node directly from the
                    SerializedProperty. You don't need to explicitly pass it in!
                  - But how do I know if that'd even be set in
                    AutoTableAttributeDrawer? Am I even working with
                    SerializedProperties in my case? I don't think I am...
                  - Nope, I'm working with OdinInspector's InspectorProperty
                    class.
                  - Could the serialized property be buried somewhere in that
                    class's properties? Hold on...
                    - ValueEntry? BaseValueEntry? Oooh, SerializationRoot sounds
                      promising...trouble is, that's also an
                      InspectorProperty...dang it.
                  - Maybe the OdinAttributeDrawer base class has some hints!
                    - It has ValueEntry...which at least will get us the values
                      that we're trying to draw. That's a good start!
                    - Can you get the object ValueEntry belongs to, perhaps?
                      That'd be an odd, but doable way of getting the Ports.
                    - ValueEntry.SmartValue is the actual list of stuff, but not
                      the ports.
                      - oh, hold on...SmartValue may have the elements, but they
                        may be separated from the Node's property at this point.
                      - Wait wait, we have access to the instance values of the
                        Attribute
                      - Can I get the property an attribute belongs to in that
                        case?
                      - ...I think we're allllll the way back to square one. I'd
                        be able to do this if I had .NET 4.5 to work with, but
                        that seems to be from THIS YEAR (2021), and the latest
                        stable unity version is from last year, so there's no
                        way their dist of .NET would be 4.5.
                      - ...2012. 
                      - .NET 4.5 was released in 2012.
                      - That's...GREAT in this case!
                      - I just tried it. It works (like, we ARE using 4.5), but the calling function
                        didn't turn out to be helpful at all.
                      - ASDLJHDFLKJHSJSLDFH
                      - 
                      - ...
                      -
                      - *sad 4:00 AM programmer noises.*


                  
          */
        }
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
