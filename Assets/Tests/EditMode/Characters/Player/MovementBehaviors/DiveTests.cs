using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using Storm.Characters.Player;
using Storm.Services;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
  public class DiveTests : StateTest<Dive> {
    
    [Test]
    public void Dive_Can_Crawl() {
      SetupTest();

      state.OnDiveFinished();
      player.CanMove().Returns(true);
      player.IsTouchingGround().Returns(true);

      state.OnFixedUpdate();

      player.Received().OnStateChange(Arg.Any<Dive>(), Arg.Any<Crawling>());
    }

    [Test]
    public void Dive_Right() {
      SetupTest();

      state.SetDiveHop(10, 10);
      physics.Vx = 1;

      state.OnStateEnter();

      Assert.AreEqual(physics.Vx, 11);
      Assert.AreEqual(physics.Vy, 10);
    }

    [Test]
    public void Dive_Left() {
      SetupTest();

      state.SetDiveHop(-10, 10);
      physics.Vx = -1;

      state.OnStateEnter();
      
      Assert.AreEqual(physics.Vx, -11);
      Assert.AreEqual(physics.Vy, 10);
    }

  }
}