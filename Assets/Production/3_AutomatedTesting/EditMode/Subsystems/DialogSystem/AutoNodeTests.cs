
using NUnit.Framework;
using NSubstitute;

using UnityEngine;

using Storm.Subsystems.Dialog;
using XNode;
using Storm.Characters.Player;
using Storm.Subsystems.Graph;

namespace Tests.Subsystems.DialogSystem {

  public class AutoNodeTests {
    private GameObject gameObject;
    private DialogManager manager;
    private IPlayer player;
    private IAutoGraph dialog;
    private IDialogBox dialogBox;
    private AutoNode node;


    private void SetupTest<NodeType>() where NodeType : AutoNode, new() {
      if (gameObject == null) {
        gameObject = new GameObject();

        manager = gameObject.AddComponent<DialogManager>();
        player = Substitute.For<IPlayer>();
        dialog = Substitute.For<IAutoGraph>();
        dialogBox = Substitute.For<IDialogBox>();

        DialogManager.Inject(player);

        node = (NodeType)ScriptableObject.CreateInstance(typeof(NodeType));
        
        DialogManager.Inject(node);
        DialogManager.Inject(dialogBox, true);
      }
    }

    // [Test]
    // public void Gets_Next_Node_Default_Behavior() {
    //   SetupTest<ActionNode>();

    //   var endNode = (EndDialogNode)ScriptableObject.CreateInstance(typeof(EndDialogNode));

    //   var outputPort = node.GetOutputPort("Output");
    //   var inputPort = endNode.GetInputPort("Input");
    //   outputPort.Connect(inputPort);

    //   DialogManager.ContinueDialog();


    //   Assert.AreEqual(null, DialogManager.GetCurrentNode());
    // }
  }
}