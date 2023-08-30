#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;

namespace TSL.SceneGraphSystem {
  public class SceneNodeView : Node {
    public SceneNode node;
    public List<Port> ports;

    public SceneNodeView(SceneNode node) {
      this.node = node;
      Debug.Log(node);
      title = node.Path.Split('/')[node.Path.Split('/').Length-1];
      this.viewDataKey = node.Key;
      style.left = node.Position.x;
      style.top = node.Position.y;
      CreatePorts();
    }

    public override void SetPosition(Rect newPos) {
      base.SetPosition(newPos);
      node.Position.x = newPos.xMin;
      node.Position.y = newPos.yMin;
    }

    private void CreatePorts() {
      ports = new List<Port>();

      Port port = InstantiatePort(
        Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool)
      );

      port.portName = "";
      inputContainer.Add(port);
      outputContainer.Add(port);
      ports.Add(port);
      
    }
  }
}
#endif