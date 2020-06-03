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
  public class CrawlingTests {

    private GameObject go;

    private Crawling state;

    private IPlayer player;

    private UnityPhysics physics;


    private void BeforeTest() {
      go = new GameObject();
      state = go.AddComponent<Crawling>();
      player = Substitute.For<IPlayer>();

      physics = go.AddComponent<UnityPhysics>();
      physics.Awake();

      state.Inject(player, physics);
    }


    // A Test behaves as an ordinary method
    [Test]
    public void Crawling_Can_Fall() {
      BeforeTest();

      player.IsTouchingGround().Returns(false);

      state.OnFixedUpdate();

      player.Received().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Jump1Fall>());
    }

    [Test]
    public void Crawling_Move_Disabled() {
      BeforeTest();

      player.CanMove().Returns(false);

      state.OnFixedUpdate();

      player.DidNotReceive().GetHorizontalInput();
    }

    [Test]
    public void Crawling_Moves_Left() {
      BeforeTest();
      
      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(-1);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      Assert.AreEqual(physics.Vx, -10);
    }

    [Test]
    public void Crawling_Faces_Left() {
      BeforeTest();
      
      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(-1);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      player.Received().SetFacing(Facing.Left);
    }

    [Test]
    public void Crawling_Moves_Right() {
      BeforeTest();
      
      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(1);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      Assert.AreEqual(physics.Vx, 10);
    }

    [Test]
    public void Crawling_Faces_Right() {
      BeforeTest();
      
      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(1);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      player.Received().SetFacing(Facing.Right);
    }

    [Test]
    public void Crawling_Stop_Move() {
      BeforeTest();

      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(0);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      Assert.AreEqual(physics.Vx, 0);
    }

    [Test]
    public void Crawling_Can_Crouch() {
      BeforeTest();

      player.IsTouchingGround().Returns(true);
      player.CanMove().Returns(true);
      player.GetHorizontalInput().Returns(0);

      state.SetCrawlSpeed(10);

      state.OnFixedUpdate();

      player.Received().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Crouching>());
    }

    [Test]
    public void Crawling_Can_Run() {
      BeforeTest();

      player.HoldingDown().Returns(false);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Running>());
    }

    [Test]
    public void Crawl_Holding_Down_Moving() {
      BeforeTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.DidNotReceive().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Running>());
    }

    [Test]
    public void Crawl_Holding_Down_Not_Moving() {
      BeforeTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(false);

      state.OnUpdate();

      player.DidNotReceive().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Running>());
    }


    [Test]
    public void Crawl_Not_Holding_Down_Not_Moving() {
      BeforeTest();

      player.HoldingDown().Returns(false);
      player.TryingToMove().Returns(false);

      state.OnUpdate();

      player.DidNotReceive().OnStateChange(Arg.Any<Crawling>(), Arg.Any<Running>());
    }
  }
}