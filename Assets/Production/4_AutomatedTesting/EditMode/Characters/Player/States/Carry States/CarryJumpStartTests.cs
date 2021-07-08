using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;

using HumanBuilders;


namespace HumanBuilders.Tests {
  public class CarryJumpStartTests : PlayerStateTest<CarryJumpStart> {
    

    [Test]
    public void Can_Throw_Item_In_Midair() {
      SetupTest();

      player.CarriedItem = BuildCarriable();
      player.ReleasedAction().Returns(true);

      state.OnUpdate();

      player.ReleasedAction().Returns(false);      
      player.PressedAction().Returns(true);
      player.HoldingAction().Returns(true);

      state.OnUpdate();
      
      AssertStateChange<MidAirThrowItem>();
    }

    [Test]
    public void Can_Rise_While_Carrying() {
      SetupTest();
      
      state.OnCarryJumpStartFinished();
      
      AssertStateChange<CarryJumpRise>();
    }

    [Test]
    public void Force_Applied() {
      SetupTest();

      settings.CarryJumpForce = 1;
      physics.Velocity = Vector2.zero;

      state.OnStateExit();

      Assert.AreEqual(1, physics.Vy);
    }
  }
}