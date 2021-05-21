using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using HumanBuilders;



namespace HumanBuilders.Tests {
  public class CrouchingTests : PlayerStateTest<Crouching> {

    [Test]
    public void Crouching_Can_End() { 
      SetupTest();

      player.HoldingDown().Returns(false);

      state.OnUpdate();

      AssertStateChange<CrouchEnd>();
    }


    [Test]
    public void Crouching_Can_Crawl_Left() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.CanCrawlLeft().Returns(true);

      state.OnUpdate();

      AssertStateChange<Crawling>();
    }

    [Test]
    public void Crouching_Can_Crawl_Right() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.CanCrawlRight().Returns(true);

      state.OnUpdate();

      AssertStateChange<Crawling>();
    }

    [Test]
    public void Crouching_Can_Prevent_Crawl() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.CanCrawlLeft().Returns(false);
      player.CanCrawlRight().Returns(false);

      state.OnUpdate();

      AssertNoStateChange<Crawling>();
    }


    [Test]
    public void Crouching_Stay_Crouching() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(false);

      state.OnUpdate();

      AssertNoStateChange<CrouchEnd>();
      AssertNoStateChange<Crawling>();
    }
  }
}