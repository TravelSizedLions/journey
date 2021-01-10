using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NSubstitute;
using UnityEngine;
using UnityEngine.TestTools;


using HumanBuilders;




namespace HumanBuilders.Tests {
  public class MidAirThrowItemTests : PlayerStateTest<MidAirThrowItem> {
    [Test]
    public void Can_Run() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.IsTouchingGround().Returns(true);
      player.TryingToMove().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<Running>();
    }


    [Test]
    public void Can_Land() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.IsTouchingGround().Returns(true);
      player.TryingToMove().Returns(false);

      state.OnFixedUpdate();


      AssertStateChange<Land>();
    }

    [Test]
    public void Can_Wall_Run() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.IsTouchingGround().Returns(false);
      player.IsTouchingLeftWall().Returns(true);
      player.IsRising().Returns(true);

      state.OnFixedUpdate();
      
      AssertStateChange<WallRun>();
    }

    [Test]
    public void Can_Wall_Slide() {
      SetupTest();
      
      player.CarriedItem = BuildCarriable();
      player.IsTouchingGround().Returns(false);
      player.IsTouchingLeftWall().Returns(true);
      player.IsRising().Returns(false);
      
      state.OnFixedUpdate();

      AssertStateChange<WallSlide>();
    }


    // [Test]
    // public void Applies_Force_Left() {
    //   SetupTest();

    //   settings.ThrowForce = Vector2.one;
    //   settings.VerticalThrowForce = 1f;

    //   Carriable c = BuildCarriable();
    //   player.CarriedItem = c;
      
    //   player.Physics.Velocity = Vector2.zero;
    //   player.Facing.Returns(Facing.Left);
    //   player.HoldingUp().Returns(false);

    //   state.OnStateEnter();

    //   Assert.AreEqual(new Vector2(-1, 1), c.Physics.Velocity);
    // }

    // [Test]
    // public void Applies_Force_Right() {
    //   SetupTest();

    //   settings.ThrowForce = Vector2.one;
    //   settings.VerticalThrowForce = 1f;

    //   Carriable c = BuildCarriable();
    //   player.CarriedItem = c;
      
    //   player.Physics.Velocity = Vector2.zero;
    //   player.Facing.Returns(Facing.Right);
    //   player.HoldingUp().Returns(false);

    //   state.OnStateEnter();

    //   Assert.AreEqual(new Vector2(1, 1), c.Physics.Velocity);
    // }

    // [Test]
    // public void Applies_Force_Up() {
    //   SetupTest();

    //   settings.ThrowForce = Vector2.one;
    //   settings.VerticalThrowForce = 1f;

    //   Carriable c = BuildCarriable();
    //   player.CarriedItem = c;
      
    //   player.Physics.Velocity = Vector2.zero;
    //   player.Facing.Returns(Facing.Left);
    //   player.HoldingUp().Returns(true);

    //   state.OnStateEnter();

    //   Assert.AreEqual(new Vector2(0, 1), c.Physics.Velocity); 
    // }

    [Test]
    public void Can_Single_Jump_Rise() {
      SetupTest();
      
      player.IsRising().Returns(true);
      state.OnMidAirThrowItemFinished();
      
      AssertStateChange<SingleJumpRise>();
    }

    [Test]
    public void Can_Single_Jump_Fall() {
      SetupTest();
      
      player.IsRising().Returns(false);
      state.OnMidAirThrowItemFinished();
      
      AssertStateChange<SingleJumpFall>();
    }
  }
}