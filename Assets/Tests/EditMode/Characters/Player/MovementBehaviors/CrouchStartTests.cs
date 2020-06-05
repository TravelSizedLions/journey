using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;

namespace Tests {
  public class CrouchStartTests : StateTest<CrouchStart> {

    [Test]
    public void CrouchStart_Can_End() {
      SetupTest();

      player.HoldingDown().Returns(false);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<CrouchStart>(), Arg.Any<CrouchEnd>());
    }

    [Test]
    public void CrouchStart_Can_Crawl() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<CrouchStart>(), Arg.Any<Crawling>());
    }

    [Test]
    public void CrouchStart_Can_Fall() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(false);
      player.IsTouchingGround().Returns(false);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<CrouchStart>(), Arg.Any<Jump1Fall>());
    }

    [Test]
    public void CrouchStart_No_Op() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(false);
      player.IsTouchingGround().Returns(true);
    }


    [Test]
    public void CrouchStart_Can_Crouch() {
      SetupTest();

      state.OnCrouchStartFinished();

      player.Received().OnStateChange(Arg.Any<CrouchStart>(), Arg.Any<Crouching>());
    }
  }
}