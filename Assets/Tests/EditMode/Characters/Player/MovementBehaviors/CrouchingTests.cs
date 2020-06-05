using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Services;

namespace Tests {
  public class CrouchingTests : StateTest<Crouching> {

    [Test]
    public void Crouching_Can_End() { 
      SetupTest();

      player.HoldingDown().Returns(false);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Crouching>(), Arg.Any<CrouchEnd>());
    }


    [Test]
    public void Crouching_Can_Crawl() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(true);

      state.OnUpdate();

      player.Received().OnStateChange(Arg.Any<Crouching>(), Arg.Any<Crawling>());
    }


    [Test]
    public void Crouching_Stay_Crouching() {
      SetupTest();

      player.HoldingDown().Returns(true);
      player.TryingToMove().Returns(false);

      state.OnUpdate();

      player.DidNotReceive().OnStateChange(Arg.Any<Crouching>(), Arg.Any<CrouchEnd>());
      player.DidNotReceive().OnStateChange(Arg.Any<Crouching>(), Arg.Any<Crawling>());
    }
  }
}