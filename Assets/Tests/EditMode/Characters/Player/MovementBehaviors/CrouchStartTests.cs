using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;

namespace Tests.Characters.Player {
  public class CrouchStartTests : PlayerStateTest<CrouchStart> {

    [Test]
    public void CrouchStart_Can_End() {
      SetupTest();

      player.HoldingDown().Returns(false);

      state.OnUpdate();

      AssertStateChange<CrouchEnd>();
    }

    [Test]
    public void CrouchStart_Can_Crawl() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(true);

      state.OnUpdate();
      AssertStateChange<Crawling>();
    }

    [Test]
    public void CrouchStart_Can_Fall() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(false);
      player.IsTouchingGround().Returns(false);

      state.OnUpdate();
      AssertStateChange<SingleJumpFall>();
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
      AssertStateChange<Crouching>();
    }
  }
}