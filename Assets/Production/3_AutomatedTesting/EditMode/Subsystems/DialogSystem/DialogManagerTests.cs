using NUnit.Framework;
using NSubstitute;

using UnityEngine;

using Storm.Subsystems.Dialog;
using XNode;
using Storm.Characters.Player;
using Storm.Subsystems.Graph;

namespace Tests.Subsystems.DialogSystem {

  public class DialogManagerTests {

    #region Fields
    private GameObject gameObject;
    private DialogManager manager;

    private GraphEngine graphEngine;

    private IPlayer player;

    private IAutoGraph dialog;

    private IDialogBox dialogBox;
    #endregion  

    #region Test Setup
    private void SetupTest() {
      if (gameObject == null) {
        gameObject = new GameObject();

        manager = gameObject.AddComponent<DialogManager>();
        player = Substitute.For<IPlayer>();
        dialog = Substitute.For<IAutoGraph>();
        dialogBox = Substitute.For<IDialogBox>();
        graphEngine = gameObject.AddComponent<GraphEngine>();

        DialogManager.Inject(player);
        DialogManager.Inject(dialogBox, false);
        DialogManager.Inject(graphEngine);
      }
    }


    private AutoGraphAsset BuildTrivialGraph() {
      AutoGraphAsset graph = new AutoGraphAsset();

      StartNode startNode = new StartNode();
      EndDialogNode endNode = new EndDialogNode();

      NodePort inPort = startNode.GetOutputPort("Output");
      NodePort outPort = endNode.GetInputPort("Input");

      inPort.AddConnections(outPort);

      return graph;
    }

    #endregion

    #region StartDialog
    [Test]
    public void StartDialog_HandlesFirstNode() {
      SetupTest();
      
      IAutoNode node = Substitute.For<IAutoNode>();
      dialog.FindStartingNode().Returns(node);

      DialogManager.StartDialog(dialog);

      node.Received().HandleNode(DialogManager.GraphEngine);
    }

    [Test]
    public void StartDialog_DisablesPlayerJump() {
      SetupTest();
      
      IAutoNode node = Substitute.For<IAutoNode>();
      dialog.FindStartingNode().Returns(node);

      DialogManager.StartDialog(dialog);

      player.Received().DisableJump();
    }

      [Test]
    public void StartDialog_DisablesPlayerMove() {
      SetupTest();
      
      IAutoNode node = Substitute.For<IAutoNode>();
      dialog.FindStartingNode().Returns(node);

      DialogManager.StartDialog(dialog);

      player.Received().DisableMove();
    }


    [Test]
    public void StartDialog_DisablesPlayerCrouch() {
      SetupTest();
      
      IAutoNode node = Substitute.For<IAutoNode>();
      dialog.FindStartingNode().Returns(node);

      DialogManager.StartDialog(dialog);

      player.Received().DisableCrouch();
    }
    #endregion

    #region EndDialog
    [Test]
    public void EndDialog_EnablesPlayerJump() {
      SetupTest();

      DialogManager.EndDialog();

      player.Received().EnableJump();
    }


    [Test]
    public void EndDialog_EnablesPlayerCrouch() {
      SetupTest();

      DialogManager.EndDialog();

      player.Received().EnableCrouch();
    }

    [Test]
    public void EndDialog_EnablesPlayerMove() {
      SetupTest();

      DialogManager.EndDialog();

      player.Received().EnableMove();
    }
    #endregion


    [Test]
    public void ContinueDialog_HandlesNode() {
      SetupTest();

      IAutoNode node = Substitute.For<IAutoNode>();
      DialogManager.Inject(dialog);
      DialogManager.Inject(node);

      DialogManager.ContinueDialog();

      node.Received().HandleNode(DialogManager.GraphEngine);
    }

  }
}