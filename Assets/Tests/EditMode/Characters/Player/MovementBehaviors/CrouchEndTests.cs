using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;

namespace Tests {
  public class CrouchEndTests {
    
    private GameObject go;

    private CrouchEnd state;

    private IPlayer player;

    private UnityPhysics physics;


    private void SetupTest() {
      go = new GameObject();
      state = go.AddComponent<CrouchEnd>();
      player = Substitute.For<IPlayer>();

      physics = go.AddComponent<UnityPhysics>();
      physics.Awake();

      state.Inject(player, physics);
    }


    [Test]
    public void CrouchEnd_Can_Run() {
      SetupTest();

      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<CrouchEnd>(), Arg.Any<Running>());
    }


    [Test]
    public void CrouchEnd_Can_Recrouch() {
      SetupTest();

      player.HoldingDown().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<CrouchEnd>(), Arg.Any<CrouchStart>());
    }


    [Test]
    public void CrouchEnd_Can_Jump() {
      SetupTest();

      player.HoldingJump().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<CrouchEnd>(), Arg.Any<Jump1Start>());
    }


    [Test]
    public void CrouchEnd_No_Op() {
      SetupTest();

      player.HoldingJump().Returns(false);
      player.HoldingDown().Returns(false);
      player.HoldingJump().Returns(false);

      player.DidNotReceive().OnStateChange(Arg.Any<CrouchEnd>(), Arg.Any<Jump1Start>());
      player.DidNotReceive().OnStateChange(Arg.Any<CrouchEnd>(), Arg.Any<CrouchStart>());
      player.DidNotReceive().OnStateChange(Arg.Any<CrouchEnd>(), Arg.Any<Running>());
    }

    [Test]
    public void CrouchEnd_Finish_Animation() {
      SetupTest();

      state.OnCrouchEndFinished();

      player.Received().OnStateChange(Arg.Any<CrouchEnd>(), Arg.Any<Idle>());
    }
  }
}