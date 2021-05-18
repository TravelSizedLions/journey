using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using HumanBuilders;



namespace HumanBuilders.Tests {
  public class CrouchStartTests : PlayerStateTest<CrouchStart> {

    [Test]
    public void CrouchStart_Can_End() {
      SetupTest();

      player.HoldingDown().Returns(false);

      state.OnUpdate();

      AssertStateChange<CrouchEnd>();
    }

    [Test]
    public void CrouchStart_Can_Crawl_Left() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(true);
      player.CanCrawlLeft().Returns(true);

      state.OnUpdate();
      AssertStateChange<Crawling>();
    }

    [Test]
    public void CrouchStart_Can_Crawl_Right() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(true);
      player.CanCrawlRight().Returns(true);

      state.OnUpdate();
      AssertStateChange<Crawling>();
    }

    [Test]
    public void CrouchStart_Interrupts_Crawl_No_Room() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.CanCrawlLeft().Returns(false);
      player.CanCrawlRight().Returns(false);

      state.OnUpdate();

      AssertNoStateChange<Crawling>();
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