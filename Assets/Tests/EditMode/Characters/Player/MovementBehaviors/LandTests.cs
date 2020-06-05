using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Storm.Characters.Player;
using Storm.Services;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
  public class LandTests : StateTest<Land> {

    [Test]
    public void Land_Can_StartCrouch() {
      SetupTest();

      player.HoldingDown().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Land>(), Arg.Any<CrouchStart>());
    }

    [Test]
    public void Land_Can_StartJump() {
      SetupTest();

      player.HoldingDown().Returns(false);
      player.PressedJump().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Land>(), Arg.Any<SingleJumpStart>());
    }

    [Test]
    public void Land_Can_Running() {
      SetupTest();

      player.HoldingDown().Returns(false);
      player.PressedJump().Returns(false);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Land>(), Arg.Any<Running>());
    }

    [Test]
    public void Land_No_Op() {
      SetupTest();

      player.HoldingDown().Returns(false);
      player.PressedJump().Returns(false);
      player.TryingToMove().Returns(false);

      state.OnUpdate();

      player.DidNotReceive().OnStateChange(Arg.Any<Land>(), Arg.Any<CrouchStart>());
      player.DidNotReceive().OnStateChange(Arg.Any<Land>(), Arg.Any<SingleJumpStart>());
      player.DidNotReceive().OnStateChange(Arg.Any<Land>(), Arg.Any<Running>());
    }

    [Test]
    public void Land_Can_Idle() {
      SetupTest();

      state.OnLandFinished();

      player.Received().OnStateChange(Arg.Any<Land>(), Arg.Any<Idle>());
    }
  }
}