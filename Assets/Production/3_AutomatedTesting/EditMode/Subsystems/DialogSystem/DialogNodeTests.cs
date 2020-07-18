
using NUnit.Framework;
using NSubstitute;

using UnityEngine;

using Storm.Subsystems.Dialog;
using XNode;
using Storm.Characters.Player;

namespace Testing.Subsystems.DialogSystem {

  public class DialogNodeTests {
    private GameObject gameObject;
    private DialogManager manager;
    private IPlayer player;
    private IDialog dialog;
    private DialogNode node;


    private void SetupTest<NodeType>() where NodeType : DialogNode, new() {
      if (gameObject == null) {
        gameObject = new GameObject();

        manager = gameObject.AddComponent<DialogManager>();
        player = Substitute.For<IPlayer>();
        dialog = Substitute.For<IDialog>();

        manager.Inject(player);

        node = (NodeType)ScriptableObject.CreateInstance(typeof(NodeType));
        
        manager.Inject(node);
        node.Inject(manager);
      }
    }


    [Test]
    public void Gets_Next_Node_Default_Behavior() {
      SetupTest<SentenceNode>();

      var endNode = (EndDialogNode)ScriptableObject.CreateInstance(typeof(EndDialogNode));

      var outputPort = node.GetOutputPort("Output");
      var inputPort = endNode.GetInputPort("Input");
      outputPort.Connect(inputPort);

      manager.ContinueDialog();

      Assert.AreEqual(endNode, manager.currentNode);
    }
  }
}