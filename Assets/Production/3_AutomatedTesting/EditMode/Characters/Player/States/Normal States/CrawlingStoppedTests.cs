using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;

using UnityEngine;
using UnityEngine.TestTools;

using Storm.Characters.Player;
using Storm.Components;
using Storm.Characters;

namespace Tests.Characters.Player {
  public class CrawlingStoppedTests : PlayerStateTest<CrawlingStopped> {

    [Test]
    public void Stopped_Can_Start_Crawling() {
      SetupTest();
      
      player.TryingToMove().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<Crawling>();
    }

    [Test]
    public void Can_Fall() {
      SetupTest();
      
      player.IsTouchingGround().Returns(false);

      state.OnFixedUpdate();
      
      AssertStateChange<SingleJumpFall>();
    }

    [Test]
    public void Can_Fall_Starts_Coyote_Time() {
      SetupTest();
      
      player.IsTouchingGround().Returns(false);
      
      state.OnFixedUpdate();

      player.Received().StartCoyoteTime();
    }
  }
}