using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

namespace TSL.SceneGraphSystem {
  public class SceneNodeView : Node {
    public SceneNode node;
    public List<Port> ports;

    public SceneNodeView(SceneNode node) {
      this.node = node;
      title = "Scene Node";
      this.viewDataKey = node.GUID;
      style.left = node.position.x;
      style.top = node.position.y;
      CreatePorts();
    }

    public override void SetPosition(Rect newPos) {
      base.SetPosition(newPos);
      node.position.x = newPos.xMin;
      node.position.y = newPos.yMin;
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