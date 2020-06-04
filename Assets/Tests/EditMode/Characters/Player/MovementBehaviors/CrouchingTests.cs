using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;

namespace Tests {
  public class CrouchingTests {

    private GameObject go;

    private Crouching state;

    private IPlayer player;

    private UnityPhysics physics;


    private void BeforeTest() {
      go = new GameObject();
      state = go.AddComponent<Crouching>();
      player = Substitute.For<IPlayer>();

      physics = go.AddComponent<UnityPhysics>();
      physics.Awake();

      state.Inject(player, physics);
    }


    [Test]
    public void Crouching_Can_End() { 
      BeforeTest();

      player.HoldingDown().Returns(false);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Crouching>(), Arg.Any<CrouchEnd>());
    }


    [Test]
    public void Crouching_Can_Crawl() {
      BeforeTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Crouching>(), Arg.Any<Crawling>());
    }


    [Test]
    public void Crouching_Stay_Crouching() {
      BeforeTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(false);

      state.OnUpdate();

      player.DidNotReceive().OnStateChange(Arg.Any<Crouching>(), Arg.Any<CrouchEnd>());
      player.DidNotReceive().OnStateChange(Arg.Any<Crouching>(), Arg.Any<Crawling>());
    }
  }
}