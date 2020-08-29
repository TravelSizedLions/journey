using NUnit.Framework;
using NSubstitute;

using UnityEngine;

using Storm.Subsystems.Dialog;
using XNode;
using Storm.Characters.Player;

namespace Tests.Subsystems.DialogSystem {

  public class DialogManagerTests {

    #region Fields
    private GameObject gameObject;
    private DialogManager manager;

    private IPlayer player;

    private IDialog dialog;

    private IDialogBox dialogBox;
    #endregion  

    #region Test Setup
    private void SetupTest() {
      if (gameObject == null) {
        gameObject = new GameObject();

        manager = gameObject.AddComponent<DialogManager>();
        player = Substitute.For<IPlayer>();
        dialog = Substitute.For<IDialog>();
        dialogBox = Substitute.For<IDialogBox>();

        manager.Inject(player);
        manager.Inject(dialogBox, false);
      }
    }


    private DialogGraph BuildTrivialGraph() {
      DialogGraph graph = new DialogGraph();

      StartDialogNode startNode = new StartDialogNode();
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
      
      IDialogNode node = Substitute.For<IDialogNode>();
      dialog.StartDialog().Returns(node);

      manager.StartDialog(dialog);

      node.Received().HandleNode();
    }

    [Test]
    public void StartDialog_DisablesPlayerJump() {
      SetupTest();
      
      IDialogNode node = Substitute.For<IDialogNode>();
      dialog.StartDialog().Returns(node);

      manager.StartDialog(dialog);

      player.Received().DisableJump();
    }

      [Test]
    public void StartDialog_DisablesPlayerMove() {
      SetupTest();
      
      IDialogNode node = Substitute.For<IDialogNode>();
      dialog.StartDialog().Returns(node);

      manager.StartDialog(dialog);

      player.Received().DisableMove();
    }


    [Test]
    public void StartDialog_DisablesPlayerCrouch() {
      SetupTest();
      
      IDialogNode node = Substitute.For<IDialogNode>();
      dialog.StartDialog().Returns(node);

      manager.StartDialog(dialog);

      player.Received().DisableCrouch();
    }
    #endregion

    #region EndDialog
    [Test]
    public void EndDialog_EnablesPlayerJump() {
      SetupTest();

      manager.EndDialog();

      player.Received().EnableJump();
    }


    [Test]
    public void EndDialog_EnablesPlayerCrouch() {
      SetupTest();

      manager.EndDialog();

      player.Received().EnableCrouch();
    }

    [Test]
    public void EndDialog_EnablesPlayerMove() {
      SetupTest();

      manager.EndDialog();

      player.Received().EnableMove();
    }
    #endregion


    [Test]
    public void ContinueDialog_HandlesNode() {
      SetupTest();

      IDialogNode node = Substitute.For<IDialogNode>();
      manager.Inject(dialog);
      manager.Inject(node);

      manager.ContinueDialog();

      node.Received().HandleNode();
    }

  }
}