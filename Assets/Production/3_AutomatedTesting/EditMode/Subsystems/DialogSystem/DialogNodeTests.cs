
using NUnit.Framework;
using NSubstitute;

using UnityEngine;

using Storm.Subsystems.Dialog;
using XNode;
using Storm.Characters.Player;

namespace Tests.Subsystems.DialogSystem {

  public class DialogNodeTests {
    private GameObject gameObject;
    private DialogManager manager;
    private IPlayer player;
    private IDialog dialog;
    private IDialogBox dialogBox;
    private DialogNode node;


    private void SetupTest<NodeType>() where NodeType : DialogNode, new() {
      if (gameObject == null) {
        gameObject = new GameObject();

        manager = gameObject.AddComponent<DialogManager>();
        player = Substitute.For<IPlayer>();
        dialog = Substitute.For<IDialog>();
        dialogBox = Substitute.For<IDialogBox>();

        manager.Inject(player);

        node = (NodeType)ScriptableObject.CreateInstance(typeof(NodeType));
        
        manager.Inject(node);
        manager.Inject(dialogBox, true);
        node.Inject(manager);
      }
    }

    [Test]
    public void Gets_Next_Node_Default_Behavior() {
      SetupTest<ActionNode>();

      var endNode = (EndDialogNode)ScriptableObject.CreateInstance(typeof(EndDialogNode));

      var outputPort = node.GetOutputPort("Output");
      var inputPort = endNode.GetInputPort("Input");
      outputPort.Connect(inputPort);

      DialogManager.ContinueDialog();


      Assert.AreEqual(null, DialogManager.GetCurrentNode());
    }
  }
}