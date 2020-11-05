using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

using NSubstitute;
using Storm.Characters.Player;

namespace Tests.Characters.Player {
  public class PickUpItemTests : PlayerStateTest<PickUpItem> {
    [Test]
    public void Can_Start_Jump_While_Carrying() {
      SetupTest();

      player.CarriedItem = BuildCarriable();

      player.PressedJump().Returns(true);
      state.OnUpdate();
      
      AssertStateChange<CarryJumpStart>();
    }

    [Test]
    public void Can_Drop_Item() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.ReleasedAction().Returns(true);
      state.OnUpdate();

      player.ReleasedAction().Returns(false);
      player.PressedAction().Returns(true);
      player.HoldingAction().Returns(true);
      state.OnUpdate();
      
      AssertStateChange<DropItem>();
    }

    [Test]
    public void Can_Run_While_Carrying() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.PressedJump().Returns(false);
      player.ReleasedAction().Returns(false);
      player.TryingToMove().Returns(true);
      state.OnUpdate();
      
      AssertStateChange<CarryRun>();
    }

    [Test]
    public void Can_Run_While_Carrying_On_Animation_Finished() {
      SetupTest();
      
      player.TryingToMove().Returns(true);
      state.OnPickupItemFinished();
      
      AssertStateChange<CarryRun>();
    }

    [Test]
    public void Can_Idle_While_Finished() {
      SetupTest();
      
      player.TryingToMove().Returns(false);
      state.OnPickupItemFinished();

      AssertStateChange<CarryIdle>();
    }
  }
}