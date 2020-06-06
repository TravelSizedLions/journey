using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;

namespace Tests.Characters.Player {
  public class CrouchingTests : PlayerStateTest<Crouching> {

    [Test]
    public void Crouching_Can_End() { 
      SetupTest();

      player.HoldingDown().Returns(false);

      state.OnUpdate();

      AssertStateChange<CrouchEnd>();
    }


    [Test]
    public void Crouching_Can_Crawl() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      AssertStateChange<Crawling>();
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