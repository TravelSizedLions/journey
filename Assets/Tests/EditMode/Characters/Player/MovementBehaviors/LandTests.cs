using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Storm.Characters.Player;
using Storm.Services;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests.Characters.Player {
  public class LandTests : PlayerStateTest<Land> {

    [Test]
    public void Land_Can_StartCrouch() {
      SetupTest();

      player.HoldingDown().Returns(true);

      state.OnUpdate();

      AssertStateChange<CrouchStart>();
    }

    [Test]
    public void Land_Can_StartJump() {
      SetupTest();

      player.HoldingDown().Returns(false);
      player.PressedJump().Returns(true);

      state.OnUpdate();

      AssertStateChange<SingleJumpStart>();
    }

    [Test]
    public void Land_Can_Running() {
      SetupTest();

      player.HoldingDown().Returns(false);
      player.PressedJump().Returns(false);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      AssertStateChange<Running>();
    }

    [Test]
    public void Land_No_Op() {
      SetupTest();

      player.HoldingDown().Returns(false);
      player.PressedJump().Returns(false);
      player.TryingToMove().Returns(false);

      state.OnUpdate();

      AssertNoStateChange<CrouchStart>();
      AssertNoStateChange<SingleJumpStart>();
      AssertNoStateChange<Running>();
    }

    [Test]
    public void Land_Can_Idle() {
      SetupTest();

      state.OnLandFinished();

      AssertStateChange<Idle>();
    }
  }
}