using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;

using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;
using Storm.Characters;

namespace Tests {
  public class CrawlingTests : StateTest<Crawling> {

    // A Test behaves as an ordinary method
    [Test]
    public void Crawling_Can_Fall() {
      SetupTest();

      player.IsTouchingGround().Returns(false);

      state.OnFixedUpdate();

      player.Received().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Jump1Fall>());
    }

    [Test]
    public void Crawling_Move_Disabled() {
      SetupTest();

      player.CanMove().Returns(false);

      state.OnFixedUpdate();

      player.DidNotReceive().GetHorizontalInput();
    }

    [Test]
    public void Crawling_Moves_Left() {
      SetupTest();
      
      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(-1);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      Assert.AreEqual(physics.Vx, -10);
    }

    [Test]
    public void Crawling_Faces_Left() {
      SetupTest();
      
      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(-1);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      player.Received().SetFacing(Facing.Left);
    }

    [Test]
    public void Crawling_Moves_Right() {
      SetupTest();
      
      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(1);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      Assert.AreEqual(physics.Vx, 10);
    }

    [Test]
    public void Crawling_Faces_Right() {
      SetupTest();
      
      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(1);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      player.Received().SetFacing(Facing.Right);
    }

    [Test]
    public void Crawling_Stop_Move() {
      SetupTest();

      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(0);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      Assert.AreEqual(physics.Vx, 0);
    }

    [Test]
    public void Crawling_Can_Crouch() {
      SetupTest();

      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(0);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      player.Received().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Crouching>());
    }

    [Test]
    public void Crawling_Can_Run() {
      SetupTest();

      player.HoldingDown().Returns(false);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Running>());
    }

    [Test]
    public void Crawl_Holding_Down_Moving() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.DidNotReceive().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Running>());
    }

    [Test]
    public void Crawl_Holding_Down_Not_Moving() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(false);

      state.OnUpdate();

      player.DidNotReceive().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Running>());
    }


    [Test]
    public void Crawl_Not_Holding_Down_Not_Moving() {
      SetupTest();

      player.HoldingDown().Returns(false);
      player.TryingToMove().Returns(false);

      state.OnUpdate();

      player.DidNotReceive().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Running>());
    }
  }
}